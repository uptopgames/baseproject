//
//  PushNotificationManager.m
//  Pushwoosh SDK
//  (c) Pushwoosh 2012
//

#import "PushNotificationManager.h"

#import "HtmlWebViewController.h"
#import "PWRequestManager.h"
#import "PWRegisterDeviceRequest.h"
#import "PWSetTagsRequest.h"
#import "PWGetTagsRequest.h"
#import "PWSendBadgeRequest.h"
#import "PWAppOpenRequest.h"
#import "PWPushStatRequest.h"
#import "PWGetNearestZoneRequest.h"
#import "PWApplicationEventRequest.h"
#import "PWSendPurchaseRequest.h"

#import "PWLocationTracker.h"

#include <sys/socket.h> // Per msqr
#include <sys/sysctl.h>
#include <net/if.h>
#include <net/if_dl.h>
#import <CommonCrypto/CommonDigest.h>

#import <AdSupport/AdSupport.h>

#define kServiceHtmlContentFormatUrl @"http://cp.pushwoosh.com/content/%@"

@interface UIApplication(Pushwoosh)
- (void) pw_setApplicationIconBadgeNumber:(NSInteger) badgeNumber;
@end

@implementation PWTags
+ (NSDictionary *) incrementalTagWithInteger:(NSInteger)delta {
	return [NSMutableDictionary dictionaryWithObjectsAndKeys:@"increment", @"operation", [NSNumber numberWithInt:delta], @"value", nil];
}
@end

@implementation PushNotificationManager

@synthesize appCode, appName, richPushWindow, pushNotifications, delegate;
@synthesize supportedOrientations, showPushnotificationAlert;

- (NSString *) stringFromMD5: (NSString *)val{
    
    if(val == nil || [val length] == 0)
        return nil;
    
    const char *value = [val UTF8String];
    
    unsigned char outputBuffer[CC_MD5_DIGEST_LENGTH];
    CC_MD5(value, strlen(value), outputBuffer);
    
    NSMutableString *outputString = [[NSMutableString alloc] initWithCapacity:CC_MD5_DIGEST_LENGTH * 2];
    for(NSInteger count = 0; count < CC_MD5_DIGEST_LENGTH; count++){
        [outputString appendFormat:@"%02x",outputBuffer[count]];
    }
    
    return [outputString autorelease];
}

////////////////////////////////////////////////////////////////////////////////
#pragma mark -
#pragma mark Private Methods

// Return the local MAC addy
// Courtesy of FreeBSD hackers email list
// Accidentally munged during previous update. Fixed thanks to erica sadun & mlamb.
- (NSString *) macaddress{
    
    int                 mib[6];
    size_t              len;
    char                *buf;
    unsigned char       *ptr;
    struct if_msghdr    *ifm;
    struct sockaddr_dl  *sdl;
    
    mib[0] = CTL_NET;
    mib[1] = AF_ROUTE;
    mib[2] = 0;
    mib[3] = AF_LINK;
    mib[4] = NET_RT_IFLIST;
    
    if ((mib[5] = if_nametoindex("en0")) == 0) {
        printf("Error: if_nametoindex error\n");
        return NULL;
    }
    
    if (sysctl(mib, 6, NULL, &len, NULL, 0) < 0) {
        printf("Error: sysctl, take 1\n");
        return NULL;
    }
    
    if ((buf = malloc(len)) == NULL) {
        printf("Could not allocate memory. error!\n");
        return NULL;
    }
    
    if (sysctl(mib, 6, buf, &len, NULL, 0) < 0) {
        printf("Error: sysctl, take 2");
        free(buf);
        return NULL;
    }
    
    ifm = (struct if_msghdr *)buf;
    sdl = (struct sockaddr_dl *)(ifm + 1);
    ptr = (unsigned char *)LLADDR(sdl);
    NSString *outstring = [NSString stringWithFormat:@"%02X:%02X:%02X:%02X:%02X:%02X", 
                           *ptr, *(ptr+1), *(ptr+2), *(ptr+3), *(ptr+4), *(ptr+5)];
    free(buf);
    
    return outstring;
}

- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions {
	for (SKPaymentTransaction *transaction in transactions) {
		if (transaction.transactionState == SKPaymentTransactionStatePurchased) {
			[self performSelectorInBackground:@selector(sendPurchaseBackground:) withObject:transaction];
		}
	}
}

////////////////////////////////////////////////////////////////////////////////
#pragma mark -
#pragma mark Public Methods

#define SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(v)  ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] != NSOrderedAscending)

- (NSString *) uniqueGlobalDeviceIdentifier{
	
	// IMPORTANT: iOS 6.0 has a bug when advertisingIdentifier or identifierForVendor occasionally might be empty! We have to fallback to hashed mac address here.
	if (SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(@"6.1")) {
		// >= iOS6 return advertisingIdentifier or identifierForVendor
		if ([NSUUID class]) {
			if ([ASIdentifierManager class]) {
				NSString *uuidString = [[ASIdentifierManager sharedManager].advertisingIdentifier UUIDString];
				if (uuidString) {
					return uuidString;
				}
			}
			
			if ([[UIDevice currentDevice] respondsToSelector:@selector(identifierForVendor)]) {
				NSString *uuidString = [[UIDevice currentDevice].identifierForVendor UUIDString];
				if (uuidString) {
					return uuidString;
				}
			}
		}
	}

	// Fallback on macaddress
    NSString *macaddress = [self macaddress];
    NSString *uniqueDeviceIdentifier = [self stringFromMD5:macaddress];
    
    return uniqueDeviceIdentifier;
}

static PushNotificationManager * instance = nil;

// this method is for backward compatibility
- (id) initWithApplicationCode:(NSString *)_appCode navController:(UIViewController *) _navController appName:(NSString *)_appName {
	return [self initWithApplicationCode:_appCode appName:_appName];
}

- (id) initWithApplicationCode:(NSString *)_appCode appName:(NSString *)_appName{
	if(self = [super init]) {
		self.supportedOrientations = PWOrientationPortrait | PWOrientationPortraitUpsideDown | PWOrientationLandscapeLeft | PWOrientationLandscapeRight;
		self.appCode = _appCode;
		self.appName = _appName;
		richPushWindow = [[UIWindow alloc] initWithFrame:[UIScreen mainScreen].bounds];
		richPushWindow.windowLevel = UIWindowLevelStatusBar + 1.0f;
		
		internalIndex = 0;
		pushNotifications = [[NSMutableDictionary alloc] init];
		showPushnotificationAlert = TRUE;
		
		[[NSUserDefaults standardUserDefaults] setObject:_appCode forKey:@"Pushwoosh_APPID"];
		if(_appName) {
			[[NSUserDefaults standardUserDefaults] setObject:_appName forKey:@"Pushwoosh_APPNAME"];
		}
		
		//initalize location tracker
		self.locationTracker = [[[PWLocationTracker alloc] init] autorelease];
		[self.locationTracker setLocationUpdatedInForeground:^ (CLLocation *location) {
			if (!location)
				return;

			[[PushNotificationManager pushManager] sendLocationBackground:location];
		}];
		
		[self.locationTracker setLocationUpdatedInBackground:^ (CLLocation *location) {
			if (!location)
				return;

			[[PushNotificationManager pushManager] sendLocationBackground:location];
		}];
		
		instance = self;
	}
	
	return self;
}

+ (void)initializeWithAppCode:(NSString *)appCode appName:(NSString *)appName {
	[[NSUserDefaults standardUserDefaults] setObject:appCode forKey:@"Pushwoosh_APPID"];
	
	if(appName) {
		[[NSUserDefaults standardUserDefaults] setObject:appName forKey:@"Pushwoosh_APPNAME"];
	}
}

+ (BOOL) getAPSProductionStatus {
	NSString * provisioning = [[NSBundle mainBundle] pathForResource:@"embedded.mobileprovision" ofType:nil];
	if(!provisioning)
		return YES;	//AppStore
	
	NSString * contents = [NSString stringWithContentsOfFile:provisioning encoding:NSASCIIStringEncoding error:nil];
	if(!contents)
		return YES;

	NSRange start = [contents rangeOfString:@"<?xml"];
	NSRange end = [contents rangeOfString:@"</plist>"];
	start.length = end.location + end.length - start.location;
	
	NSString * profile =[contents substringWithRange:start];
	if(!profile)
		return YES;
	
	NSData * profileData = [profile dataUsingEncoding:NSUTF8StringEncoding];
	NSString *error = nil;;
	NSPropertyListFormat format;
	NSDictionary* plist = [NSPropertyListSerialization propertyListFromData:profileData mutabilityOption:NSPropertyListImmutable format:&format errorDescription:&error];
	
	NSDictionary * entitlements = [plist objectForKey:@"Entitlements"];
//	NSNumber * allowNumber = [entitlements objectForKey:@"get-task-allow"];
	
	//could be development or production
	NSString * apsGateway = [entitlements objectForKey:@"aps-environment"];
	
	if(!apsGateway) {
		UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Pushwoosh Error" message:@"Your provisioning profile does not have APS entry. Please make your profile push compatible." delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil, nil];
		[alert show];
		[alert release];
	}
	
	if([apsGateway isEqualToString:@"development"])
		return NO;
	
	return YES;
}

+ (NSString *) getAppIdFromBundle:(BOOL)productionAPS {
	NSString * appid = nil;
	if(!productionAPS) {
		appid = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"Pushwoosh_APPID_Dev"];
		if(appid)
			return appid;
	}
	
	return [[NSBundle mainBundle] objectForInfoDictionaryKey:@"Pushwoosh_APPID"];
}

+ (PushNotificationManager *)pushManager {
	if(instance == nil) {
		NSString * appid = [self getAppIdFromBundle:[self getAPSProductionStatus]];
		
		if(!appid) {
			appid = [[NSUserDefaults standardUserDefaults] objectForKey:@"Pushwoosh_APPID"];

			if(!appid) {
				return nil;
			}
		}
		
		NSString * appname = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"Pushwoosh_APPNAME"];
		if(!appname)
			appname = [[NSUserDefaults standardUserDefaults] objectForKey:@"Pushwoosh_APPNAME"];
		
		if(!appname)
			appname = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleDisplayName"];
		
		if(!appname)
			appname = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleName"];
			
		if(!appname) {
			appname = @"";
		}
		
		instance = [[PushNotificationManager alloc] initWithApplicationCode:appid appName:appname ];
	}
	
	return instance;
}

- (void) showWebView {
	self.richPushWindow.alpha = 0.0f;
	self.richPushWindow.windowLevel = UIWindowLevelStatusBar + 1.0f;
	self.richPushWindow.hidden = NO;
	self.richPushWindow.transform = CGAffineTransformMakeScale(0.01, 0.01);
	
	[UIView animateWithDuration:0.3 animations:^{
		self.richPushWindow.transform = CGAffineTransformIdentity;
		self.richPushWindow.alpha = 1.0f;
	} completion:^(BOOL finished) {
	}];

}

- (void) showPushPage:(NSString *)pageId {
	NSString *url = [NSString stringWithFormat:kServiceHtmlContentFormatUrl, pageId];
	HtmlWebViewController *vc = [[HtmlWebViewController alloc] initWithURLString:url];
	vc.delegate = self;
	vc.supportedOrientations = supportedOrientations;

	self.richPushWindow.rootViewController = vc;
	[vc view];
	[vc release];
}

- (void) showCustomPushPage:(NSString *)page {
	HtmlWebViewController *vc = [[HtmlWebViewController alloc] initWithURLString:page];
	vc.delegate = self;
	vc.supportedOrientations = supportedOrientations;
	
	self.richPushWindow.rootViewController = vc;
	[vc view];
	[vc release];
}

- (void)htmlWebViewControllerDidClose:(HtmlWebViewController *)viewController {
	
	self.richPushWindow.transform = CGAffineTransformIdentity;
	[UIView animateWithDuration:0.3 delay:0 options:UIViewAnimationOptionCurveEaseOut animations:^{
		self.richPushWindow.transform = CGAffineTransformMakeScale(0.01, 0.01);
		self.richPushWindow.alpha = 0.0f;
	} completion:^(BOOL finished) {
		self.richPushWindow.hidden = YES;

	}];
}

- (void) sendDevTokenToServer:(NSString *)deviceID {
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
	NSString * appLocale = @"en";
	NSLocale * locale = (NSLocale *)CFLocaleCopyCurrent();
	NSString * localeId = [locale localeIdentifier];
	
	if([localeId length] > 2)
		localeId = [localeId stringByReplacingCharactersInRange:NSMakeRange(2, [localeId length]-2) withString:@""];
	
	[locale release]; locale = nil;
	
	appLocale = localeId;
	
	NSArray * languagesArr = (NSArray *) CFLocaleCopyPreferredLanguages();	
	if([languagesArr count] > 0)
	{
		NSString * value = [languagesArr objectAtIndex:0];
		
		if([value length] > 2)
			value = [value stringByReplacingCharactersInRange:NSMakeRange(2, [value length]-2) withString:@""];
		
		appLocale = [[value copy] autorelease];
	}
	
	[languagesArr release]; languagesArr = nil;
	
	PWRegisterDeviceRequest *request = [[PWRegisterDeviceRequest alloc] init];
	request.appId = appCode;
	request.hwid = [self uniqueGlobalDeviceIdentifier];
	request.pushToken = deviceID;
	request.language = appLocale;
	request.timeZone = [NSString stringWithFormat:@"%d", [[NSTimeZone localTimeZone] secondsFromGMT]];
	
	NSError *error = nil;
	if ([[PWRequestManager sharedManager] sendRequest:request error:&error]) {
		NSLog(@"Registered for push notifications: %@", deviceID);

		if([delegate respondsToSelector:@selector(onDidRegisterForRemoteNotificationsWithDeviceToken:)] ) {
			[delegate performSelectorOnMainThread:@selector(onDidRegisterForRemoteNotificationsWithDeviceToken:) withObject:[self getPushToken] waitUntilDone:NO];
		}
	} else {
		NSLog(@"Registered for push notifications failed");

		if([delegate respondsToSelector:@selector(onDidFailToRegisterForRemoteNotificationsWithError:)] ) {
			[delegate performSelectorOnMainThread:@selector(onDidFailToRegisterForRemoteNotificationsWithError:) withObject:error waitUntilDone:NO];
		}
	}
	
	[request release]; request = nil;
	[pool release]; pool = nil;
}

- (void) handlePushRegistrationString:(NSString *)deviceID {
	
	[[NSUserDefaults standardUserDefaults] setObject:deviceID forKey:@"PWPushUserId"];
	
	[self performSelectorInBackground:@selector(sendDevTokenToServer:) withObject:deviceID];
}

- (void) handlePushRegistration:(NSData *)devToken {
	NSMutableString *deviceID = [NSMutableString stringWithString:[devToken description]];
	
	//Remove <, >, and spaces
	[deviceID replaceOccurrencesOfString:@"<" withString:@"" options:1 range:NSMakeRange(0, [deviceID length])];
	[deviceID replaceOccurrencesOfString:@">" withString:@"" options:1 range:NSMakeRange(0, [deviceID length])];
	[deviceID replaceOccurrencesOfString:@" " withString:@"" options:1 range:NSMakeRange(0, [deviceID length])];
	
	[[NSUserDefaults standardUserDefaults] setObject:deviceID forKey:@"PWPushUserId"];
	
	[self performSelectorInBackground:@selector(sendDevTokenToServer:) withObject:deviceID];
}

- (void) handlePushRegistrationFailure:(NSError *) error {
	if([delegate respondsToSelector:@selector(onDidFailToRegisterForRemoteNotificationsWithError:)] ) {
		[delegate performSelectorOnMainThread:@selector(onDidFailToRegisterForRemoteNotificationsWithError:) withObject:error waitUntilDone:NO];
	}
}

- (NSString *) getPushToken {
	return [[NSUserDefaults standardUserDefaults] objectForKey:@"PWPushUserId"];
}

#pragma mark URL redirect handling flow

//This method is added to work with shorten urls
//According to ios 6, if user isn't logged in appstore, then when safari opens itunes url system will ask permission to run appstore.
//But still if application open appstore url, system will open it without any alerts.
- (void) openUrl: (NSURL *) url {
	//When opening nsurlconnection to some url if it has some redirect, then connection will ask delegate what to do.
	//But if url has no redirects, then THIS CODE WILL NOT WORK.
	//
	//Pushwoosh.com guarantee that any http/https url is shorten URL.
	//Unshort url and open it by usual way.
	if ([[url scheme] hasPrefix:@"http"]) {
		NSURLConnection *connection  = [[NSURLConnection alloc] initWithRequest:[NSMutableURLRequest requestWithURL:url] delegate:self];
		if (!connection) {
			return;
		}
		
		[connection release];
		return;
	}
	
	//If url has cusmtom scheme like facebook:// or itms:// we need to open it directly:
	[[UIApplication sharedApplication] openURL:url];
}

- (void)connection:(NSURLConnection *)connection didReceiveResponse:(NSURLResponse *)response {
	NSLog(@"Url: %@", [response URL]);

	//as soon as all the redirects finished we can open the final URL
	[[UIApplication sharedApplication] openURL:[response URL]];
}

#pragma mark -
- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex {
	if(buttonIndex != 1) {
		if(!alertView.tag)
			return;
		
		[pushNotifications removeObjectForKey:[NSNumber numberWithInt:alertView.tag]];
		return;
	}
	
	NSDictionary *lastPushDict = [pushNotifications objectForKey:[NSNumber numberWithInt:alertView.tag]];
	NSString *htmlPageId = [lastPushDict objectForKey:@"h"];
	if(htmlPageId) {
		[self showPushPage:htmlPageId];
	}

	NSString *customHtmlPageId = [lastPushDict objectForKey:@"r"];
	if(customHtmlPageId) {
		[self showCustomPushPage:customHtmlPageId];
	}
    
	NSString *linkUrl = [lastPushDict objectForKey:@"l"];	
	if(linkUrl) {
		[self openUrl:[NSURL URLWithString:linkUrl]];
	}
	
	if([delegate respondsToSelector:@selector(onPushAccepted: withNotification:)] ) {
		[delegate onPushAccepted:self withNotification:lastPushDict];
	}
	else
	if([delegate respondsToSelector:@selector(onPushAccepted: withNotification: onStart:)] ) {
		[delegate onPushAccepted:self withNotification:lastPushDict onStart:NO];
	}
	
	[pushNotifications removeObjectForKey:[NSNumber numberWithInt:alertView.tag]];
}

- (BOOL) handlePushReceived:(NSDictionary *)userInfo {
	
	BOOL isPushOnStart = NO;
	NSDictionary *pushDict = [userInfo objectForKey:@"aps"];
	if(!pushDict) {
		//try as launchOptions dictionary
		userInfo = [userInfo objectForKey:UIApplicationLaunchOptionsRemoteNotificationKey];
		pushDict = [userInfo objectForKey:@"aps"];
		isPushOnStart = YES;
	}
	
	if (!pushDict)
		return NO;
	
	//check if the app is really running
	if([[UIApplication sharedApplication] respondsToSelector:@selector(applicationState)] && [[UIApplication sharedApplication] applicationState] != UIApplicationStateActive) {
		isPushOnStart = YES;
	}
	
	NSString *hash = [userInfo objectForKey:@"p"];
	
	[self performSelectorInBackground:@selector(sendStatsBackground:) withObject:hash];
	
	if([delegate respondsToSelector:@selector(onPushReceived: withNotification: onStart:)] ) {
		[delegate onPushReceived:self withNotification:userInfo onStart:isPushOnStart];
		return YES;
	}

	NSString *alertMsg = [pushDict objectForKey:@"alert"];
	
	bool msgIsString = YES;
	if(![alertMsg isKindOfClass:[NSString class]])
		msgIsString = NO;
	
//	NSString *badge = [pushDict objectForKey:@"badge"];
//	NSString *sound = [pushDict objectForKey:@"sound"];
	NSString *htmlPageId = [userInfo objectForKey:@"h"];
//	NSString *customData = [userInfo objectForKey:@"u"];
	NSString *linkUrl = [userInfo objectForKey:@"l"];
	NSString *customHtmlPageId = [userInfo objectForKey:@"r"];
	
	//the app is running, display alert only
	if(!isPushOnStart && showPushnotificationAlert && msgIsString) {
		UIAlertView *alert = [[UIAlertView alloc] initWithTitle:self.appName message:alertMsg delegate:self cancelButtonTitle:@"Cancel" otherButtonTitles:@"OK", nil];
		alert.tag = ++internalIndex;
		[pushNotifications setObject:userInfo forKey:[NSNumber numberWithInt:internalIndex]];
		[alert show];
		[alert release];
		return YES;
	}
	
	if(htmlPageId) {
		[self showPushPage:htmlPageId];
	}

	if(customHtmlPageId) {
		[self showCustomPushPage:customHtmlPageId];
	}
    
	if(linkUrl) {
		[self openUrl:[NSURL URLWithString:linkUrl]];
	}
	
	if([delegate respondsToSelector:@selector(onPushAccepted: withNotification:)] ) {
		[delegate onPushAccepted:self withNotification:userInfo];
	}
	else
	if([delegate respondsToSelector:@selector(onPushAccepted: withNotification: onStart:)] ) {
		[delegate onPushAccepted:self withNotification:userInfo onStart:isPushOnStart];
	}

	return YES;
}

- (NSDictionary *) getApnPayload:(NSDictionary *)pushNotification {
	return [pushNotification objectForKey:@"aps"];
}

- (NSString *) getCustomPushData:(NSDictionary *)pushNotification {
	return [pushNotification objectForKey:@"u"];
}

- (void) sendStatsBackground:(NSString *)hash {
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];

	PWPushStatRequest *request = [[PWPushStatRequest alloc] init];
	request.appId = appCode;
	request.hash = hash;
	request.hwid = [self uniqueGlobalDeviceIdentifier];
	
	if ([[PWRequestManager sharedManager] sendRequest:request]) {
		NSLog(@"sendStats completed");
	} else {
		NSLog(@"sendStats failed");
	}
	
	[request release]; request = nil;
	
	[pool release]; pool = nil;
}

- (void) sendTagsBackground: (NSDictionary *) tags {
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];

    PWSetTagsRequest *request = [[PWSetTagsRequest alloc] init];
	request.appId = appCode;
	request.hwid = [self uniqueGlobalDeviceIdentifier];
    request.tags = tags;
	
	if ([[PWRequestManager sharedManager] sendRequest:request]) {
		NSLog(@"setTags completed");
	} else {
		NSLog(@"setTags failed");
	}
	
	[request release]; request = nil;

	
	[pool release]; pool = nil;
}

- (void) sendLocationBackground: (CLLocation *) location {
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
	NSLog(@"Sending location: %@", location);
	
    PWGetNearestZoneRequest *request = [[PWGetNearestZoneRequest alloc] init];
	request.appId = appCode;
	request.hwid = [self uniqueGlobalDeviceIdentifier];
    request.coordinate = location.coordinate;
	
	if ([[PWRequestManager sharedManager] sendRequest:request]) {
		NSLog(@"getNearestZone completed");
	} else {
		NSLog(@"getNearestZone failed");
	}
	
	[request release]; request = nil;
	
	NSLog(@"Locaiton sent");
	
	[pool release]; pool = nil;
}

- (void) sendLocation: (CLLocation *) location {
	[self performSelectorInBackground:@selector(sendLocationBackground:) withObject:location];
}

- (void) sendAppOpenBackground {
	//it's ok to call this method without push token
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
	PWAppOpenRequest *request = [[PWAppOpenRequest alloc] init];
	request.appId = appCode;
	request.hwid = [self uniqueGlobalDeviceIdentifier];
	
	if ([[PWRequestManager sharedManager] sendRequest:request]) {
		NSLog(@"sending appOpen completed");
	} else {
		NSLog(@"sending appOpen failed");
	}
	
	[request release]; request = nil;
	[pool release]; pool = nil;
}

- (void) sendBadgesBackground: (NSNumber *) badge {
	if([[PushNotificationManager pushManager] getPushToken] == nil)
		return;
	
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];

	PWSendBadgeRequest *request = [[PWSendBadgeRequest alloc] init];
	request.appId = appCode;
	request.hwid = [self uniqueGlobalDeviceIdentifier];
	request.badge = [badge intValue];
		
	if ([[PWRequestManager sharedManager] sendRequest:request]) {
		NSLog(@"setBadges completed");
	} else {
		NSLog(@"setBadges failed");
	}
	
	[request release]; request = nil;
	[pool release]; pool = nil;
}

- (void) sendGoalBackground: (PWApplicationEventRequest *) request {
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
	if ([[PWRequestManager sharedManager] sendRequest:request]) {
		NSLog(@"sendGoals completed");
	} else {
		NSLog(@"sendGoals failed");
	}

	[pool release]; pool = nil;
}

- (void)sendPurchaseBackground:(SKPaymentTransaction *)transaction {
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
	PWSendPurchaseRequest *purchaseRequest = [[PWSendPurchaseRequest alloc] init];
	purchaseRequest.appId = appCode;
	purchaseRequest.hwid = [self uniqueGlobalDeviceIdentifier];
	purchaseRequest.productIdentifier = transaction.payment.productIdentifier;
	purchaseRequest.quantity = transaction.payment.quantity;
	purchaseRequest.transactionDate = transaction.transactionDate;
	
	if ([[PWRequestManager sharedManager] sendRequest:purchaseRequest]) {
		NSLog(@"sendPurchase completed");
	} else {
		NSLog(@"sendPurchase failed");
	}
	
	[purchaseRequest release];
	
	[pool release];
}

- (void) sendBadges: (NSInteger) badge {
	[self performSelectorInBackground:@selector(sendBadgesBackground:) withObject:[NSNumber numberWithInt:badge]];
}

- (void) sendAppOpen {
	[self performSelectorInBackground:@selector(sendAppOpenBackground) withObject:nil];
}

- (void) setTags: (NSDictionary *) tags {
	[self performSelectorInBackground:@selector(sendTagsBackground:) withObject:tags];
}

- (void) loadTags {
	[self loadTags:nil error:nil];
}

- (void) loadTags: (pushwooshGetTagsHandler) successHandler error:(pushwooshErrorHandler) errorHandler{
	dispatch_queue_t queue = dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0ul);
	dispatch_async(queue, ^{
		PWGetTagsRequest *request = [[PWGetTagsRequest alloc] init];
		request.appId = appCode;
		request.hwid = [self uniqueGlobalDeviceIdentifier];
		
		NSError *error = nil;
		if ([[PWRequestManager sharedManager] sendRequest:request error:&error]) {
			NSLog(@"loadTags completed");
			
			dispatch_async(dispatch_get_main_queue(), ^{
				if([delegate respondsToSelector:@selector(onTagsReceived:)] ) {
					[delegate onTagsReceived:request.tags];
				}
				
				if(successHandler) {
					successHandler(request.tags);
				}
			});
			
		} else {
			NSLog(@"loadTags failed");
			
			dispatch_async(dispatch_get_main_queue(), ^{
				if([delegate respondsToSelector:@selector(onTagsFailedToReceive:)] ) {
					[delegate onTagsFailedToReceive:error];
				}
				
				if(errorHandler) {
					errorHandler(error);
				}
			});
		}
		
		[request release]; request = nil;
	});
}

- (void) recordGoal: (NSString *) goal {
	[self recordGoal:goal withCount:nil];
}

- (void) recordGoal: (NSString *) goal withCount: (NSNumber *) count {
	PWApplicationEventRequest *request = [[PWApplicationEventRequest alloc] init];
	request.appId = appCode;
	request.hwid = [self uniqueGlobalDeviceIdentifier];
	request.goal = goal;
	request.count = count;

	[self performSelectorInBackground:@selector(sendGoalBackground:) withObject:request];
	[request release];
}

//clears the notifications from the notification center
+ (void) clearNotificationCenter {
	
	UIApplication* application = [UIApplication sharedApplication];
	NSArray* scheduledNotifications = [NSArray arrayWithArray:application.scheduledLocalNotifications];
	application.scheduledLocalNotifications = scheduledNotifications;
}

//start location tracking. this is battery efficient and uses network triangulation in background
- (void)startLocationTracking {
	NSString *modeString = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"Pushwoosh_BGMODE"];
	[self startLocationTracking:modeString];
}

- (void) startLocationTracking:(NSString *)mode {
	self.locationTracker.backgroundMode = mode;
	self.locationTracker.enabled = YES;
}

//stops location tracking
- (void) stopLocationTracking {
	self.locationTracker.enabled = NO;
}

- (void) dealloc {
	self.richPushWindow = nil;
	self.delegate = nil;
	self.appCode = nil;
	self.pushNotifications = nil;
	
	[super dealloc];
}

@end
