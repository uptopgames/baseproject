//
//  EtceteraManager.h
//  EtceteraTest
//
//  Created by Mike on 10/2/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <MessageUI/MessageUI.h>
#import <MessageUI/MFMailComposeViewController.h>


typedef enum
{
	PhotoTypeCamera,
	PhotoTypeAlbum,
	PhotoTypeBoth
} PhotoType;


@interface EtceteraManager : NSObject <UIAlertViewDelegate, MFMailComposeViewControllerDelegate, MFMessageComposeViewControllerDelegate, UINavigationControllerDelegate, UIImagePickerControllerDelegate, UIActionSheetDelegate>
{
@private
	id _popoverViewController;
	
	float _hoursBetweenPrompts;
}
@property (nonatomic, retain) NSString *urbanAirshipAppKey;
@property (nonatomic, retain) NSString *urbanAirshipAppSecret;
@property (nonatomic, retain) NSString *urbanAirshipAlias;
@property (nonatomic, retain) NSString *iTunesUrl;
@property (nonatomic, assign) float scaledImageSize;
@property (nonatomic, assign) CGRect popoverRect;
@property (nonatomic, assign) BOOL pickerAllowsEditing;
@property (nonatomic, assign) float JPEGCompression;
@property (nonatomic, retain) id popoverViewController;

@property (nonatomic, retain) UIView *keyboardView;
@property (nonatomic, retain) UIColor *borderColor;
@property (nonatomic, retain) UIColor *gradientStopOne;
@property (nonatomic, retain) UIColor *gradientStopTwo;
@property (nonatomic, retain) UIWebView *inlineWebView;


+ (EtceteraManager*)sharedManager;

+ (NSString*)stringWithNewUUID;

+ (UIViewController*)unityViewController;

+ (NSString*)jsonFromObject:(id)object;

+ (id)objectFromJson:(NSString*)json;

- (void)dismissWrappedController;


// UIAlertView
- (void)showAlertWithTitle:(NSString*)title message:(NSString*)message buttons:(NSArray*)buttons;

- (void)showPromptWithTitle:(NSString*)title message:(NSString*)message placeHolder:(NSString*)placeHolder autocorrect:(BOOL)autocorrect;

- (void)showPromptWithTitle:(NSString*)title message:(NSString*)message placeHolder1:(NSString*)placeHolder1 placeHolder2:(NSString*)placeHolder2 autocorrect:(BOOL)autocorrect;


// P31WebController
- (void)showWebControllerWithUrl:(NSString*)url showingControls:(BOOL)showControls;


// Mail
- (BOOL)isEmailAvailable;

- (BOOL)isSMSAvailable;

- (void)showMailComposerWithTo:(NSString*)toAddress subject:(NSString*)subject body:(NSString*)body isHTML:(BOOL)isHTML;

- (void)showMailComposerWithTo:(NSString*)toAddress subject:(NSString*)subject body:(NSString*)body isHTML:(BOOL)isHTML attachment:(NSData*)data mimeType:(NSString*)mimeType filename:(NSString*)filename;

- (void)showSMSComposerWithBody:(NSString*)body;

- (void)showSMSComposerWithRecipients:(NSArray*)recipients body:(NSString*)body;

- (void)showSMSComposerWithBody:(NSString*)body;

- (void)showSMSComposerWithRecipients:(NSArray*)recipients body:(NSString*)body;


// Rate this app
- (void)askForReviewWithLaunchCount:(int)launchCount hoursBetweenPrompts:(float)hoursBetweenPrompts title:(NSString*)title message:(NSString*)message iTunesAppId:(NSString*)iTunesAppId;

- (void)askForReviewWithTitle:(NSString*)title message:(NSString*)message iTunesAppId:(NSString*)iTunesAppId;


// Photo and Photo Library
- (void)promptForPhotoWithType:(PhotoType)type;


// Inline web view
- (void)inlineWebViewShowWithFrame:(CGRect)frame;

- (void)inlineWebViewClose;

- (void)inlineWebViewSetUrl:(NSString*)urlString;

- (void)inlineWebViewSetFrame:(CGRect)frame;

@end




@interface UIImage(OrientationAdditions)

- (UIImage*)imageWithImageDataMatchingOrientation;

@end
