//
//  EtceteraManager.m
//  EtceteraTest
//
//  Created by Mike on 10/2/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import "EtceteraManager.h"
#import "P31WebController.h"
#include <sys/socket.h>
#include <sys/sysctl.h>
#include <net/if.h>
#include <net/if_dl.h>
#import <CommonCrypto/CommonDigest.h>


void UnityPause( bool pause );

void UnitySendMessage( const char * className, const char * methodName, const char * param );

UIViewController *UnityGetGLViewController();


UIColor * ColorFromHex( int hexcolor )
{
	int r = ( hexcolor >> 24 ) & 0xFF;
	int g = ( hexcolor >> 16 ) & 0xFF;
	int b = ( hexcolor >> 8 ) & 0xFF;
	int a = hexcolor & 0xFF;

	return [UIColor colorWithRed:(r/255.0) green:(g/255.0) blue:(b/255.0) alpha:(a/255.0)];
}


// UIAlertView tags
#define kStandardAlertTag		1111
#define kSingleFieldAlertTag	2222
#define kTwoFieldAlertTag		3333
#define kRTAAlertTag			4444
#define kRTAAlertTagNoOptions	7777

// RTA defaults keys
#define kRTADontAskAgain			@"RTADontAskAgain"
#define kRTALastReviewedVersion		@"RTALastReviewedVersion"
#define kRTANextTimeToAsk			@"RTANextTimeToAsk"
#define kRTATimesLaunchedSinceAsked	@"RTATimesLaunchedSinceAsked"



@implementation EtceteraManager

@synthesize urbanAirshipAppKey = _urbanAirshipAppKey, urbanAirshipAppSecret = _urbanAirshipAppSecret, iTunesUrl = _iTunesUrl, scaledImageSize = _scaledImageSize,
			borderColor = _borderColor, gradientStopOne = _gradientStopOne, gradientStopTwo = _gradientStopTwo,
			popoverRect, pickerAllowsEditing = _pickerAllowsEditing, popoverViewController = _popoverViewController, urbanAirshipAlias,
			inlineWebView;

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Class Methods

+ (NSString*)stringWithNewUUID
{
    // Create a new UUID
    CFUUIDRef uuidObj = CFUUIDCreate( nil );
    
    // Get the string representation of the UUID
    NSString *newUUID = (NSString*)CFUUIDCreateString( nil, uuidObj );
    CFRelease( uuidObj );
    return [newUUID autorelease];
}


+ (UIViewController*)unityViewController
{
	return UnityGetGLViewController();
}


+ (NSString*)jsonFromObject:(id)object
{
	NSError *error = nil;
	NSData *jsonData = [NSJSONSerialization dataWithJSONObject:object options:0 error:&error];
	
	if( jsonData && !error )
	{
		return [[[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding] autorelease];
	}
	else
	{
		NSLog( @"json serialization error: %@", [error localizedDescription] );
	}
	
	return @"{}";
}


+ (id)objectFromJson:(NSString*)json
{
	NSData *jsonData = [json dataUsingEncoding:NSUTF8StringEncoding];
	if( jsonData )
	{
		return [NSJSONSerialization JSONObjectWithData:jsonData options:0 error:nil];
	}
	else
	{
		NSLog( @"jsonData was null when converted from the passed in string" );
	}
	
    return [NSDictionary dictionary];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

+ (EtceteraManager*)sharedManager
{
	static EtceteraManager *sharedManager = nil;
	
	if( !sharedManager )
		sharedManager = [[EtceteraManager alloc] init];
	
	return sharedManager;
}


- (id)init
{
	if( ( self = [super init] ) )
	{
		_JPEGCompression = 0.8;
		_pickerAllowsEditing = NO;
		_scaledImageSize = 1.0f;
		popoverRect = CGRectMake( 20, 15, 10, 0 );
	}
	return self;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Private

- (void)showViewControllerModallyInWrapper:(UIViewController*)viewController
{
	// pause the game
	UnityPause( true );
	
	// cancel the previous delayed call to dismiss the view controller if it exists
	[NSObject cancelPreviousPerformRequestsWithTarget:self];

	UIViewController *vc = UnityGetGLViewController();
	
	// show the mail composer on iPad in a form sheet
	if( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad && [viewController isKindOfClass:[MFMailComposeViewController class]] )
		viewController.modalPresentationStyle = UIModalPresentationFormSheet;
	
	// show the view controller
	[vc presentModalViewController:viewController animated:YES];
}


- (void)dismissWrappedController
{
	UnityPause( false );

	UIViewController *vc = UnityGetGLViewController();
	
	// No view controller? Get out of here.
	if( !vc )
		return;
	
	// dismiss the view controller
	[vc dismissModalViewControllerAnimated:YES];

	// remove the wrapper view controller
	[self performSelector:@selector(removeAndReleaseViewControllerWrapper) withObject:nil afterDelay:1.0];
	
	UnitySendMessage( "EtceteraManager", "dismissingViewController", "" );
}


- (void)removeAndReleaseViewControllerWrapper
{
	// iPad might have a popover
	if( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad && _popoverViewController )
	{
		[_popoverViewController dismissPopoverAnimated:YES];
		self.popoverViewController = nil;
	}
}


- (NSString*)iTunesUrlForAppId:(NSString*)appId
{
	return [NSString stringWithFormat:@"itms-apps://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=%@", appId];
}


- (void)image:(UIImage*)image didFinishSavingWithError:(NSError*)error contextInfo:(void*)contextInfo
{
	NSLog( @"image:didFinishSavingWithError:contextInfo: completed" );
	
	if( error )
	{
		NSLog( @"image:didFinishSavingWithError:contextInfo: %@", error );
		UnitySendMessage( "EtceteraManager", "saveImageToPhotoAlbumFailed", error.localizedDescription.UTF8String );
	}
	else
	{
		UnitySendMessage( "EtceteraManager", "saveImageToPhotoAlbumSucceeded", "" );
	}

}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Public

// UIAlertView
- (void)showAlertWithTitle:(NSString*)title message:(NSString*)message buttons:(NSArray*)buttons
{
	UnityPause( true );
	UIAlertView *alert = [[[UIAlertView alloc] init] autorelease];
	alert.delegate = self;
	alert.title = title;
	alert.message = message;
	
	for( NSString *b in buttons )
		[alert addButtonWithTitle:b];
	
	alert.tag = kStandardAlertTag;
	[alert show];
}


- (void)showPromptWithTitle:(NSString*)title message:(NSString*)message placeHolder:(NSString*)placeHolder autocorrect:(BOOL)autocorrect
{
	UnityPause( true );
	
	// we can use the fancy new Alertview if we are on iOS 5+
	UIAlertView *alertView = [[[UIAlertView alloc] initWithTitle:title message:message delegate:self cancelButtonTitle:NSLocalizedString( @"Cancel", nil ) otherButtonTitles:NSLocalizedString( @"OK", nil ), nil] autorelease];
	alertView.alertViewStyle = UIAlertViewStylePlainTextInput;
	alertView.tag = kSingleFieldAlertTag;
	
	// configure the text field
	UITextField *tf = [alertView textFieldAtIndex:0];
	tf.placeholder = placeHolder;
	if( !autocorrect )
		tf.autocorrectionType = UITextAutocorrectionTypeNo;
	
	[alertView show];
}


- (void)showPromptWithTitle:(NSString*)title message:(NSString*)message placeHolder1:(NSString*)placeHolder1 placeHolder2:(NSString*)placeHolder2 autocorrect:(BOOL)autocorrect
{
	UnityPause( true );
	
	// we can use the fancy new Alertview if we are on iOS 5+
	UIAlertView *alertView = [[[UIAlertView alloc] initWithTitle:title
														 message:message
														delegate:self
											   cancelButtonTitle:NSLocalizedString( @"Cancel", nil )
											   otherButtonTitles:NSLocalizedString( @"OK", nil ), nil] autorelease];
	alertView.alertViewStyle = UIAlertViewStyleLoginAndPasswordInput;
	alertView.tag = kTwoFieldAlertTag;
	
	// configure the text field
	[alertView textFieldAtIndex:0].placeholder = placeHolder1;
	[alertView textFieldAtIndex:1].placeholder = placeHolder2;
	
	if( !autocorrect )
	{
		[alertView textFieldAtIndex:0].autocorrectionType = UITextAutocorrectionTypeNo;
		[alertView textFieldAtIndex:1].autocorrectionType = UITextAutocorrectionTypeNo;
	}
	
	// If the second placeHolder has 'password' in it, make it a password field
	[alertView textFieldAtIndex:1].secureTextEntry = [placeHolder2 hasPrefix:@"password"];
	
	[alertView show];
}


// P31WebController
- (void)showWebControllerWithUrl:(NSString*)url showingControls:(BOOL)showControls
{
	UnityPause( true );
	
	P31WebController *webCon = [[P31WebController alloc] initWithUrl:url showControls:showControls];
	UINavigationController *navCon = [[UINavigationController alloc] initWithRootViewController:webCon];
	[self showViewControllerModallyInWrapper:navCon];
	[navCon release];
	[webCon release];
}


// Mail and SMS
- (BOOL)isEmailAvailable
{
	return [MFMailComposeViewController canSendMail];
}


- (BOOL)isSMSAvailable
{
	Class composerClass = NSClassFromString( @"MFMessageComposeViewController" );
	
	if( !composerClass )
		return NO;
	
	return [composerClass canSendText];
}


- (void)showMailComposerWithTo:(NSString*)toAddress subject:(NSString*)subject body:(NSString*)body isHTML:(BOOL)isHTML
{
	[self showMailComposerWithTo:toAddress
						 subject:subject
							body:body
						  isHTML:isHTML
					  attachment:nil
						mimeType:nil
						filename:nil];
}


- (void)showMailComposerWithTo:(NSString*)toAddress subject:(NSString*)subject body:(NSString*)body isHTML:(BOOL)isHTML attachment:(NSData*)data mimeType:(NSString*)mimeType filename:(NSString*)filename
{
	// early out if email isnt setup
	if( ![self isEmailAvailable] )
		return;
	
	MFMailComposeViewController *mailer = [[MFMailComposeViewController alloc] init];
	mailer.mailComposeDelegate = self;
	
	[mailer setSubject:subject];
	[mailer setMessageBody:body isHTML:isHTML];
	
	// Add the to address if we have one and it has an '@'
	if( toAddress && toAddress.length && [toAddress rangeOfString:@"@"].location != NSNotFound )
		[mailer setToRecipients:[NSArray arrayWithObject:toAddress]];
	
	// Add the attachment if we have one
	if( data && filename && mimeType )
		[mailer addAttachmentData:data mimeType:mimeType fileName:filename];
	
	[self showViewControllerModallyInWrapper:mailer];
}


- (void)showMailComposerWithTo:(NSString*)toAddress subject:(NSString*)subject body:(NSString*)body isHTML:(BOOL)isHTML imageAttachment:(NSData*)imageData
{
	// early out if email isnt setup
	if( ![self isEmailAvailable] )
		return;
	
	MFMailComposeViewController *mailer = [[MFMailComposeViewController alloc] init];
	mailer.mailComposeDelegate = self;
	
	[mailer setSubject:subject];
	[mailer setMessageBody:body isHTML:isHTML];
	
	// Add the to address if we have one and it has an '@'
	if( toAddress && toAddress.length && [toAddress rangeOfString:@"@"].location != NSNotFound )
		[mailer setToRecipients:[NSArray arrayWithObject:toAddress]];
	
	// Add the attachment if we have one
	if( imageData )
		[mailer addAttachmentData:imageData mimeType:@"image/png" fileName:@"image.png"];
	
	[self showViewControllerModallyInWrapper:mailer];
}


- (void)showSMSComposerWithBody:(NSString*)body
{
	[self showSMSComposerWithRecipients:nil body:body];
}


- (void)showSMSComposerWithRecipients:(NSArray*)recipients body:(NSString*)body
{
	if( ![self isSMSAvailable] )
		return;
	
	[UIApplication sharedApplication].statusBarHidden = NO;

	MFMessageComposeViewController *controller = [[MFMessageComposeViewController alloc] init];
	controller.body = body;
	controller.recipients = recipients;
	controller.messageComposeDelegate = self;
	
	[self showViewControllerModallyInWrapper:controller];
}


// Rate This App

// checks for if the user asked not to be asked to review the app, makes sure it has been 2 days since last asking and checks to see if the app
// version was already reviewed
- (BOOL)isAppEligibleForReviewWithLaunchCount:(int)launchCount
{
	NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
	
	// If the user doesnt want us to ever ask this question than dont ask
	if( [defaults boolForKey:kRTADontAskAgain] )
		return NO;
	
	// Grab the current version from the bundle and the last reviewed version
	NSString *currentVersion = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleVersion"];
	NSString *lastReviewedVersion = [defaults stringForKey:kRTALastReviewedVersion];
	
	// If this version has been reviewed, than get out of here
	if( [lastReviewedVersion isEqualToString:currentVersion] )
		return NO;
	
	// Take care of setting the launch count and keeping it up to date
	int count = [defaults integerForKey:kRTATimesLaunchedSinceAsked];
	[defaults setInteger:++count forKey:kRTATimesLaunchedSinceAsked];
	
	// see if we were launched enough times yet
	if( count < launchCount )
		return NO;
	
	// If we don't have a next time to ask yet, set it for 2 days from now.  We never prompt on the first run
	const double currentTime = CFAbsoluteTimeGetCurrent();
	if( [defaults objectForKey:kRTANextTimeToAsk] == nil )
	{
		const double nextTime = currentTime + _hoursBetweenPrompts * 60 * 60;
		[defaults setDouble:nextTime forKey:kRTANextTimeToAsk];
		return NO;
	}
	
	// Grab the next time we should ask and see if we are good to bug the user again
	const double nextTime = [defaults doubleForKey:kRTANextTimeToAsk];
	if( currentTime > nextTime )
		return YES;
	
	return NO;
}


// will ask for review if: the user didnt click 'dont ask again', it has been at least 2 days since last asking or since the first launch,
// the current version has not been reviewed and the app has been launched more than launchCount times since the last review
- (void)askForReviewWithLaunchCount:(int)launchCount hoursBetweenPrompts:(float)hoursBetweenPrompts title:(NSString*)title message:(NSString*)message iTunesAppId:(NSString*)iTunesAppId
{
	// store this globally for easy access
	_hoursBetweenPrompts = hoursBetweenPrompts;
	
	// early out if we don't pass the isEligible test
	if( ![self isAppEligibleForReviewWithLaunchCount:launchCount] )
		return;
	
	UIAlertView *alert = [[UIAlertView alloc] initWithTitle:title
													message:message
												   delegate:self
										  cancelButtonTitle:NSLocalizedString( @"Remind me later", nil )
										  otherButtonTitles:NSLocalizedString( @"Yes, rate it!", nil ), NSLocalizedString( @"Don't ask again", nil ), nil];
	alert.tag = kRTAAlertTag;
	[alert show];
	[alert release];
	
	// Save the iTunesUrl for now
	self.iTunesUrl = [self iTunesUrlForAppId:iTunesAppId];
}


// will ask for review no matter what
- (void)askForReviewWithTitle:(NSString*)title message:(NSString*)message iTunesAppId:(NSString*)iTunesAppId
{
	UIAlertView *alert = [[UIAlertView alloc] initWithTitle:title
													message:message
												   delegate:self
										  cancelButtonTitle:NSLocalizedString( @"Cancel", nil )
										  otherButtonTitles:NSLocalizedString( @"OK!", nil ), nil];
	alert.tag = kRTAAlertTagNoOptions;
	[alert show];
	[alert release];
	
	// Save the iTunesUrl for now
	self.iTunesUrl = [self iTunesUrlForAppId:iTunesAppId];
}


// Photo Library and Camera
- (void)showPicker:(UIImagePickerControllerSourceType)type
{
	UIImagePickerController *picker = [[[UIImagePickerController alloc] init] autorelease];
	picker.delegate = self;
	picker.sourceType = type;
	picker.allowsEditing = _pickerAllowsEditing;
	
	// We need to display this in a popover on iPad
	if( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad )
	{
		Class popoverClass = NSClassFromString( @"UIPopoverController" );
		if( !popoverClass )
			return;

		_popoverViewController = [[popoverClass alloc] initWithContentViewController:picker];
		[_popoverViewController setDelegate:self];
		//picker.modalInPopover = YES;
		
		// Display the popover
		[_popoverViewController presentPopoverFromRect:popoverRect
												inView:UnityGetGLViewController().view
							  permittedArrowDirections:UIPopoverArrowDirectionAny
											  animated:YES];
	}
	else
	{
		// wrap and show the modal
		[self showViewControllerModallyInWrapper:picker];
	}
}


- (void)popoverControllerDidDismissPopover:(UIPopoverController*)popoverController
{
	self.popoverViewController = nil;
	UnityPause( false );
	
	UnitySendMessage( "EtceteraManager", "imagePickerDidCancel", "" );
}


- (void)promptForPhotoWithType:(PhotoType)type
{
	UnityPause( true );

	// No need to give a choice for devices with no camera
	if( ![UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera] )
	{
		[self showPicker:UIImagePickerControllerSourceTypePhotoLibrary];
		return;
	}
	
	if( type == PhotoTypeAlbum )
	{
		[self showPicker:UIImagePickerControllerSourceTypePhotoLibrary];
		return;
	}
	else if( type == PhotoTypeCamera )
	{
		[self showPicker:UIImagePickerControllerSourceTypeCamera];
		return;
	}
	
	UIActionSheet *sheet = [[UIActionSheet alloc] initWithTitle:nil
													   delegate:self
											  cancelButtonTitle:NSLocalizedString( @"Cancel", nil )
										 destructiveButtonTitle:nil
											  otherButtonTitles:NSLocalizedString( @"Take Photo", nil ), NSLocalizedString( @"Choose Existing Photo", nil ), nil];
	
	if( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad )
		[sheet showFromRect:popoverRect inView:UnityGetGLViewController().view animated:YES];
	else
		[sheet showInView:UnityGetGLViewController().view];
	
	[sheet release];
}


// Inline web view
- (void)inlineWebViewShowWithFrame:(CGRect)frame
{
	if( inlineWebView )
		[self inlineWebViewClose];
	
	inlineWebView = [[UIWebView alloc] initWithFrame:frame];
	inlineWebView.scalesPageToFit = YES;
	inlineWebView.autoresizingMask = UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;
	[UnityGetGLViewController().view addSubview:inlineWebView];
}


- (void)inlineWebViewClose
{
	[inlineWebView removeFromSuperview];
	self.inlineWebView = nil;
}


- (void)inlineWebViewSetUrl:(NSString*)urlString
{
	NSURLRequest *request = [NSURLRequest requestWithURL:[NSURL URLWithString:urlString]];
	[inlineWebView loadRequest:request];
}


- (void)inlineWebViewSetFrame:(CGRect)frame
{
	[UIView beginAnimations:nil context:NULL];
	inlineWebView.frame = frame;
	[UIView commitAnimations];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark UIActionSheetDelegate

- (void)actionSheet:(UIActionSheet*)actionSheet clickedButtonAtIndex:(NSInteger)buttonIndex
{
	if( buttonIndex == 0 )
	{
		[self showPicker:UIImagePickerControllerSourceTypeCamera];
	}
	else if( buttonIndex == 1 )
	{
		[self showPicker:UIImagePickerControllerSourceTypePhotoLibrary];
	}
	else // Cancelled
	{
		UnityPause( false );
		UnitySendMessage( "EtceteraManager", "imagePickerDidCancel", "" );
	}
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark UIImagePickerControllerDelegate

- (void)imagePickerController:(UIImagePickerController*)picker didFinishPickingMediaWithInfo:(NSDictionary*)info
{
	// Grab the image and write it to disk
	UIImage *image;
	
	if( _pickerAllowsEditing )
		image = [info objectForKey:UIImagePickerControllerEditedImage];
	else
		image = [info objectForKey:UIImagePickerControllerOriginalImage];
	
	NSLog( @"picker got image with orientation: %i", image.imageOrientation );

	// Do the save and resize on a background thread if we are on iOS 4 > (UIKit is threadsafe there)
	if( NULL != &UIGraphicsBeginImageContextWithOptions )
		[self performSelectorInBackground:@selector(processImageFromImagePicker:) withObject:image];
	else
		[self performSelector:@selector(processImageFromImagePicker:) withObject:image];

	// Dimiss the pickerController
	[self dismissWrappedController];
}


- (void)processImageFromImagePicker:(UIImage*)image
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
	// Get a filepath pointing to the docs directory
	NSArray *dirs = NSSearchPathForDirectoriesInDomains( NSDocumentDirectory, NSUserDomainMask, YES );
	NSString *filename = [NSString stringWithFormat:@"%@.jpg", [EtceteraManager stringWithNewUUID]];
	NSString *filePath = [[dirs objectAtIndex:0] stringByAppendingPathComponent:filename];
	
	// Shrink the monster image down
	if( _scaledImageSize != 1.0f )
	{
		float width = image.size.width * _scaledImageSize;
		float height = image.size.height * _scaledImageSize;
		CGSize targetSize = CGSizeMake( width, height );
		UIGraphicsBeginImageContext( targetSize );
		[image drawInRect:CGRectMake( 0, 0, targetSize.width, targetSize.height )];
		UIImage *targetImage = UIGraphicsGetImageFromCurrentImageContext();
		UIGraphicsEndImageContext();
		
		image = targetImage;
	}

	[UIImageJPEGRepresentation( [image imageWithImageDataMatchingOrientation], _JPEGCompression ) writeToFile:filePath atomically:NO];
	
	[self performSelectorOnMainThread:@selector(notifyUnityOfSavedImageAtPath:) withObject:filePath waitUntilDone:NO];
	
	[pool release];
}


- (void)notifyUnityOfSavedImageAtPath:(NSString*)filePath
{
	// Message back to Unity
	UnitySendMessage( "EtceteraManager", "imageSavedToDocuments", [filePath UTF8String] );	
}


- (void)imagePickerControllerDidCancel:(UIImagePickerController*)picker
{
	// dismiss the wrapper, unpause and notifiy Unity what happened
	[self dismissWrappedController];
	UnityPause( false );
	UnitySendMessage( "EtceteraManager", "imagePickerDidCancel", "" );
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark UIAlertViewDelegate

- (void)alertView:(UIAlertView*)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
	UnityPause( false );

	// always dump the button clicked
	NSString *title = [alertView buttonTitleAtIndex:buttonIndex];
	UnitySendMessage( "EtceteraManager", "alertViewClickedButton", [title UTF8String] );

	if( alertView.tag == kRTAAlertTag || alertView.tag == kRTAAlertTagNoOptions ) // Rate this app. We can share with the no options version
	{
		switch( buttonIndex )
		{
			case 0: // remind me later
			{
				const double nextTime = CFAbsoluteTimeGetCurrent() + _hoursBetweenPrompts * 60 * 60;
				[[NSUserDefaults standardUserDefaults] setDouble:nextTime forKey:kRTANextTimeToAsk];
				break;
			}
			case 1: // rate it now
			{
				// grab the current version and save it in the defaults
				NSString *version = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleVersion"];
				[[NSUserDefaults standardUserDefaults] setValue:version forKey:kRTALastReviewedVersion];
				
				[[UIApplication sharedApplication] openURL:[NSURL URLWithString:self.iTunesUrl]];
				break;
			}
			case 2: // don't ask again
			{
				[[NSUserDefaults standardUserDefaults] setBool:YES forKey:kRTADontAskAgain];
				break;
			}
		}
		
		// release the url and reset the launch count
		self.iTunesUrl = nil;
		[[NSUserDefaults standardUserDefaults] setInteger:0 forKey:kRTATimesLaunchedSinceAsked];
	}
	else if( alertView.tag == kSingleFieldAlertTag || alertView.tag == kTwoFieldAlertTag ) // single field prompt
	{
		if( buttonIndex == 0 )
		{
			UnitySendMessage( "EtceteraManager", "alertPromptCancelled", "" );
		}
		else
		{
			NSString *returnText;
			if( alertView.tag == kSingleFieldAlertTag )
				returnText = [alertView textFieldAtIndex:0].text;
			else
				returnText = [NSString stringWithFormat:@"%@|||%@", [alertView textFieldAtIndex:0].text, [alertView textFieldAtIndex:1].text];
			
			UnitySendMessage( "EtceteraManager", "alertPromptEnteredText", [returnText UTF8String] );
		}
	}
}
						   


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark MFMailComposerDelegate

- (void)mailComposeController:(MFMailComposeViewController*)controller didFinishWithResult:(MFMailComposeResult)result error:(NSError*)error
{
	[self dismissWrappedController];
	
	NSString *resultString = nil;
	
	switch( result )
	{
		case MFMailComposeResultCancelled:
			resultString = @"Cancelled";
			break;
		case MFMailComposeResultSaved:
			resultString = @"Saved";
			break;
		case MFMailComposeResultSent:
			resultString = @"Sent";
			break;
		case MFMailComposeResultFailed:
			resultString = @"Failed";
			break;
		default:
			resultString = @"";
	}
	
	UnitySendMessage( "EtceteraManager", "mailComposerFinishedWithResult", [resultString UTF8String] );
	
	// autorelease this after 2 seconds to avoid an odd crash when another mail composer is presented
	[controller performSelector:@selector(autorelease) withObject:nil afterDelay:2.0];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark MFMessageComposeViewControllerDelegate

- (void)messageComposeViewController:(MFMessageComposeViewController*)controller didFinishWithResult:(MessageComposeResult)result
{
	[self dismissWrappedController];
	
	NSString *resultString = nil;
	
	switch( result )
	{
		case MessageComposeResultCancelled:
			resultString = @"Cancelled";
			break;
		case MessageComposeResultSent:
			resultString = @"Sent";
			break;
		case MessageComposeResultFailed:
			resultString = @"Failed";
			break;
		default:
			resultString = @"";
	}
	
	UnitySendMessage( "EtceteraManager", "smsComposerFinishedWithResult", [resultString UTF8String] );
	
	// autorelease this after 2 seconds to avoid an odd crash when another SMS composer is presented
	[controller performSelector:@selector(autorelease) withObject:nil afterDelay:3.0];
	
	[UIApplication sharedApplication].statusBarHidden = YES;
}



@end




@implementation UIImage(OrientationAdditions)

- (UIImage*)imageWithImageDataMatchingOrientation
{
    // no-op if the orientation is already correct
    if( self.imageOrientation == UIImageOrientationUp )
		return self;
	
    // We need to calculate the proper transformation to make the image upright.
    // We do it in 2 steps: Rotate if Left/Right/Down, and then flip if Mirrored.
    CGAffineTransform transform = CGAffineTransformIdentity;
	
    switch( self.imageOrientation )
	{
        case UIImageOrientationDown:
        case UIImageOrientationDownMirrored:
            transform = CGAffineTransformTranslate( transform, self.size.width, self.size.height );
            transform = CGAffineTransformRotate( transform, M_PI );
            break;
			
        case UIImageOrientationLeft:
        case UIImageOrientationLeftMirrored:
            transform = CGAffineTransformTranslate( transform, self.size.width, 0 );
            transform = CGAffineTransformRotate( transform, M_PI_2 );
            break;
			
        case UIImageOrientationRight:
        case UIImageOrientationRightMirrored:
            transform = CGAffineTransformTranslate( transform, 0, self.size.height );
            transform = CGAffineTransformRotate( transform, -M_PI_2 );
            break;
    }
	
    switch( self.imageOrientation )
	{
        case UIImageOrientationUpMirrored:
        case UIImageOrientationDownMirrored:
            transform = CGAffineTransformTranslate( transform, self.size.width, 0 );
            transform = CGAffineTransformScale( transform, -1, 1 );
            break;
			
        case UIImageOrientationLeftMirrored:
        case UIImageOrientationRightMirrored:
            transform = CGAffineTransformTranslate( transform, self.size.height, 0 );
            transform = CGAffineTransformScale( transform, -1, 1 );
            break;
    }
	
    // Now we draw the underlying CGImage into a new context, applying the transform calculated above.
    CGContextRef ctx = CGBitmapContextCreate( NULL, self.size.width, self.size.height,
                                             CGImageGetBitsPerComponent( self.CGImage ), 0,
                                             CGImageGetColorSpace( self.CGImage ),
                                             CGImageGetBitmapInfo( self.CGImage ) );
    CGContextConcatCTM( ctx, transform );
    switch( self.imageOrientation )
	{
        case UIImageOrientationLeft:
        case UIImageOrientationLeftMirrored:
        case UIImageOrientationRight:
        case UIImageOrientationRightMirrored:
            CGContextDrawImage( ctx, CGRectMake( 0, 0, self.size.height, self.size.width ), self.CGImage );
            break;
			
        default:
            CGContextDrawImage( ctx, CGRectMake( 0, 0, self.size.width, self.size.height ), self.CGImage );
            break;
    }
	
    // And now we just create a new UIImage from the drawing context
    CGImageRef cgimg = CGBitmapContextCreateImage(ctx);
    UIImage *img = [UIImage imageWithCGImage:cgimg];
    CGContextRelease(ctx);
    CGImageRelease(cgimg);
	
    return img;
}

@end