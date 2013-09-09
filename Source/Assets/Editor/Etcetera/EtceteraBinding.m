//
//  EtceteraBinding.m
//  EtceteraTest
//
//  Created by Mike on 10/2/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//
#import "EtceteraManager.h"
#import "P31ActivityView.h"


#if UNITY_VERSION < 420
	#import "AppController.h"
	#define APPCONTROLLER_CLASS AppController
#else
	#import "UnityAppController.h"
	#define APPCONTROLLER_CLASS UnityAppController
#endif


// Converts NSString to C style string by way of copy (Mono will free it)
#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

// Converts C style string to NSString as long as it isnt empty
#define GetStringParamOrNil( _x_ ) ( _x_ != NULL && strlen( _x_ ) ) ? [NSString stringWithUTF8String:_x_] : nil


BOOL _etceteraApplicationCanOpenUrl( const char * url )
{
	return [[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:GetStringParam( url )]];
}


// Localization
const char * _etceteraGetCurrentLanguage()
{
	NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
	NSArray *languages = [defaults objectForKey:@"AppleLanguages"];
	
	return MakeStringCopy( [languages objectAtIndex:0] );
}


const char * _etceteraLocaleObjectForKey( BOOL useAutoUpdatingLocale, const char * key )
{
	NSLocale *locale = useAutoUpdatingLocale ? [NSLocale autoupdatingCurrentLocale] : [NSLocale currentLocale];
	id res = [locale objectForKey:GetStringParam( key )];
	if( [res isKindOfClass:[NSString class]] )
	{
		return MakeStringCopy( res );
	}
	
	return NULL;
}


const char * _etceteraGetLocalizedString( const char * key, const char * defaultValue )
{
	NSString *result = [[NSBundle mainBundle] localizedStringForKey:GetStringParam( key ) value:GetStringParam( defaultValue ) table:nil];
	return MakeStringCopy( result );
}


// UIAlertView
void _etceteraShowAlertWithTitleMessageAndButtons( const char * title, const char * message, const char * buttons )
{
	NSArray *buttonArray = [EtceteraManager objectFromJson:GetStringParam( buttons )];
	[[EtceteraManager sharedManager] showAlertWithTitle:GetStringParam( title )
												message:GetStringParam( message )
												buttons:buttonArray];
}


void _etceteraShowPromptWithOneField( const char * title, const char * message, const char * placeHolder, bool autocorrect )
{
	[[EtceteraManager sharedManager] showPromptWithTitle:GetStringParam( title )
												 message:GetStringParam( message )
											 placeHolder:GetStringParam( placeHolder )
											 autocorrect:autocorrect];
}


void _etceteraShowPromptWithTwoFields( const char * title, const char * message, const char * placeHolder1, const char * placeHolder2, bool autocorrect )
{
	[[EtceteraManager sharedManager] showPromptWithTitle:GetStringParam( title )
												 message:GetStringParam( message )
											 placeHolder1:GetStringParam( placeHolder1 )
											placeHolder2:GetStringParam( placeHolder2 )
											 autocorrect:autocorrect];
}


// Web
void _etceteraShowWebPage( const char * url, bool showControls )
{
	[[EtceteraManager sharedManager] showWebControllerWithUrl:GetStringParam( url ) showingControls:showControls];
}


// Mail and SMS
bool _etceteraIsEmailAvailable()
{
	return [[EtceteraManager sharedManager] isEmailAvailable];
}


bool _etceteraIsSMSAvailable()
{
	return [[EtceteraManager sharedManager] isSMSAvailable];
}


void _etceteraShowMailComposer( const char * toAddress, const char * subject, const char * body, bool isHTML )
{
	[[EtceteraManager sharedManager] showMailComposerWithTo:GetStringParam( toAddress )
													subject:GetStringParam( subject )
													   body:GetStringParam( body )
													 isHTML:isHTML];
}


void _etceteraShowMailComposerWithRawAttachment( UInt8 *bytes, int length, const char * attachmentMimeType, const char * attachmentFilename, const char * toAddress, const char * subject, const char * body, bool isHTML )
{
	NSData *data = [[NSData alloc] initWithBytes:(void*)bytes length:length];
	[[EtceteraManager sharedManager] showMailComposerWithTo:GetStringParam( toAddress )
													subject:GetStringParam( subject )
													   body:GetStringParam( body )
													 isHTML:isHTML
												 attachment:data
												   mimeType:GetStringParam( attachmentMimeType )
												   filename:GetStringParam( attachmentFilename )];
}


void _etceteraShowSMSComposer( const char * recipients, const char * body )
{
	NSArray *recipientArray = [EtceteraManager objectFromJson:GetStringParam( recipients )];
	[[EtceteraManager sharedManager] showSMSComposerWithRecipients:recipientArray body:GetStringParam( body )];
}


// Activity View
void _etceteraHideActivityView()
{
	[P31ActivityView removeView];
}


void _etceteraShowActivityView()
{
	[P31ActivityView newActivityView];
}


void _etceteraShowActivityViewWithLabel( const char * label )
{
	[P31ActivityView newActivityViewWithLabel:GetStringParam( label )];
}


void _etceteraShowBezelActivityViewWithLabel( const char * label )
{
	[P31BezelActivityView newActivityViewWithLabel:GetStringParam( label )];
}


void _etceteraShowBezelActivityViewWithImage( const char * label, const char * imagePath )
{
	UIImage *image = [UIImage imageWithContentsOfFile:GetStringParam( imagePath )];
	[P31ImageActivityView newActivityViewWithLabel:GetStringParam( label ) withImage:image];
}


// Rate this app
void _etceteraAskForReview( int launchCount, float hoursBetweenPrompts, const char * title, const char * message, const char * iTunesAppId )
{
	[[EtceteraManager sharedManager] askForReviewWithLaunchCount:launchCount
											 hoursBetweenPrompts:hoursBetweenPrompts
														   title:GetStringParam( title )
														 message:GetStringParam( message )
													   iTunesAppId:GetStringParam( iTunesAppId )];
}


void _etceteraAskForReviewImmediately( const char * title, const char * message, const char * iTunesAppId )
{
	[[EtceteraManager sharedManager] askForReviewWithTitle:GetStringParam( title )
												   message:GetStringParam( message )
												 iTunesAppId:GetStringParam( iTunesAppId )];
}


// Photo and Library
void _etceteraSetPopoverPoint( float xPos, float yPos )
{
	[EtceteraManager sharedManager].popoverRect = CGRectMake( xPos, yPos, 10, 10 );
}


void _etceteraPromptForPhoto( float scaledToSize, int promptType, float jpegCompression, BOOL allowsEditing )
{
	[EtceteraManager sharedManager].JPEGCompression = jpegCompression;
	[EtceteraManager sharedManager].pickerAllowsEditing = allowsEditing;
	[EtceteraManager sharedManager].scaledImageSize = scaledToSize;
	[[EtceteraManager sharedManager] promptForPhotoWithType:(PhotoType)promptType];
}


void _etceteraResizeImageAtPath( const char * filePath, float width, float height )
{
	NSString *fullImagePath = GetStringParam( filePath );

	// early out if the file doesnt exist
	if( ![[NSFileManager defaultManager] fileExistsAtPath:fullImagePath] )
		return;

	UIImage *image = [UIImage imageWithContentsOfFile:fullImagePath];

	// early out if we dont have in image
	if( !image )
		return;

	// Shrink the monster image down
	CGSize targetSize = CGSizeMake( width, height );
	UIGraphicsBeginImageContext( targetSize );
	[image drawInRect:CGRectMake( 0, 0, targetSize.width, targetSize.height )];
	UIImage *targetImage = UIGraphicsGetImageFromCurrentImageContext();
	UIGraphicsEndImageContext();

	[UIImageJPEGRepresentation( targetImage, [EtceteraManager sharedManager].JPEGCompression ) writeToFile:fullImagePath atomically:NO];
}


const char * _etceteraGetImageSize( const char * filePath )
{
	NSString *fullImagePath = GetStringParam( filePath );

	// early out if the file doesnt exist
	if( ![[NSFileManager defaultManager] fileExistsAtPath:fullImagePath] )
		return MakeStringCopy( @"0,0" );

	UIImage *i = [[UIImage alloc] initWithContentsOfFile:fullImagePath];
	NSString *size = [NSString stringWithFormat:@"%.0f,%.0f", i.size.width, i.size.height];
	[i release];

	return MakeStringCopy( size );


	/* iOS 4 ONLY and requires ImageIO framework */
	/*
	NSURL *imageFileURL = [NSURL fileURLWithPath:fullImagePath];
	CGImageSourceRef imageSource = CGImageSourceCreateWithURL( (CFURLRef)imageFileURL, NULL );
	if( !imageSource )
		return MakeStringCopy( @"0,0" );

	CGFloat width = 0.0f, height = 0.0f;
	CFDictionaryRef imageProperties = CGImageSourceCopyPropertiesAtIndex( imageSource, 0, NULL );
	if (imageProperties != NULL) {
		CFNumberRef widthNum  = CFDictionaryGetValue( imageProperties, kCGImagePropertyPixelWidth );
		if( widthNum != NULL )
			CFNumberGetValue( widthNum, kCFNumberFloatType, &width );

		CFNumberRef heightNum = CFDictionaryGetValue( imageProperties, kCGImagePropertyPixelHeight );
		if( heightNum != NULL )
			CFNumberGetValue( heightNum, kCFNumberFloatType, &height );

		CFRelease( imageProperties );
	}

	return [NSString stringWithFormat:@"%.0f,%.0f", width, height );
	*/
}


void _etceteraSaveImageToPhotoAlbum( const char * filePath )
{
	NSString *fullImagePath = GetStringParam( filePath );

	// early out if the file doesnt exist
	if( ![[NSFileManager defaultManager] fileExistsAtPath:fullImagePath] )
		return;

	UIImage *image = [UIImage imageWithContentsOfFile:fullImagePath];

	if( image )
		UIImageWriteToSavedPhotosAlbum( image, [EtceteraManager sharedManager], @selector(image:didFinishSavingWithError:contextInfo:), NULL );
}


// Push
void _etceteraSetUrbanAirshipCredentials( const char * appKey, const char * appSecret, const char *alias )
{
	[EtceteraManager sharedManager].urbanAirshipAppKey = GetStringParam( appKey );
	[EtceteraManager sharedManager].urbanAirshipAppSecret = GetStringParam( appSecret );
	[EtceteraManager sharedManager].urbanAirshipAlias = GetStringParamOrNil( alias );
}


void _etceteraRegisterForRemoteNotifications( int types )
{
	if( [APPCONTROLLER_CLASS respondsToSelector:@selector(registerForRemoteNotificationTypes:)] )
		[APPCONTROLLER_CLASS performSelector:@selector(registerForRemoteNotificationTypes:) withObject:[NSNumber numberWithInt:types]];
	else
		NSLog( @"The Application delegate does not respond to registerForRemoteNotificationTypes:. Did you remove the AppControllerPushAdditions file and forget to readd it?" );
}


int _etceteraGetEnabledRemoteNotificationTypes()
{
	if( [APPCONTROLLER_CLASS respondsToSelector:@selector(enabledRemoteNotificationTypes)] )
	{
		NSNumber *num = [APPCONTROLLER_CLASS performSelector:@selector(enabledRemoteNotificationTypes)];
		return [num intValue];
	}
	else
	{
		NSLog( @"The Application delegate does not respond to enabledRemoteNotificationTypes. Did you remove the AppControllerPushAdditions file and forget to readd it?" );
	}

	return -1;
}


int _etceteraGetBadgeCount()
{
	return [UIApplication sharedApplication].applicationIconBadgeNumber;
}


void _etceteraSetBadgeCount( int badgeCount )
{
	[UIApplication sharedApplication].applicationIconBadgeNumber = badgeCount;
}


int _etceteraGetStatusBarOrientation()
{
	return [UIApplication sharedApplication].statusBarOrientation;
}


// Inline web view
void _etceteraInlineWebViewShow( int x, int y, int width, int height )
{
	[[EtceteraManager sharedManager] inlineWebViewShowWithFrame:CGRectMake( x, y, width, height)];
}


void _etceteraInlineWebViewClose()
{
	[[EtceteraManager sharedManager] inlineWebViewClose];
}


void _etceteraInlineWebViewSetUrl( const char * url )
{
	[[EtceteraManager sharedManager] inlineWebViewSetUrl:GetStringParam( url )];
}


void _etceteraInlineWebViewSetFrame( int x, int y, int width, int height )
{
	[[EtceteraManager sharedManager] inlineWebViewSetFrame:CGRectMake( x, y, width, height)];
}

