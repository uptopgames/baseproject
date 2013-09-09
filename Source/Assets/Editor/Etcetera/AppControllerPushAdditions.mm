//
//  AppControllerPushAdditions.m
//  EtceteraTest
//
//  Created by Mike on 10/5/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import "AppControllerPushAdditions.h"
#import "EtceteraManager.h"


void UnitySendMessage( const char * className, const char * methodName, const char * param );
void UnitySendDeviceToken( NSData* deviceToken );
void UnitySendRemoteNotification( NSDictionary* notification );
void UnitySendRemoteNotificationError( NSError* error );
void UnitySendLocalNotification( UILocalNotification* notification );


#if UNITY_VERSION < 420

@implementation AppController(PushAdditions)

#else

@implementation UnityAppController(PushAdditions)

#endif


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - Class methods

+ (void)load
{
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(applicationDidFinishLaunchingNotification:) name:UIApplicationDidFinishLaunchingNotification object:nil];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - NSNotifications

+ (void)applicationDidFinishLaunchingNotification:(NSNotification*)note
{
	if( note.userInfo )
	{
		NSDictionary *remoteNotificationDictionary = [note.userInfo objectForKey:UIApplicationLaunchOptionsRemoteNotificationKey];
		if( remoteNotificationDictionary )
		{
			NSLog( @"launched with remote notification: %@", remoteNotificationDictionary );
			double delayInSeconds = 5.0;
			dispatch_time_t popTime = dispatch_time( DISPATCH_TIME_NOW, (int64_t)(delayInSeconds * NSEC_PER_SEC) );
			dispatch_after( popTime, dispatch_get_main_queue(), ^
			{
				APPCONTROLLER_CLASS *appCon = (APPCONTROLLER_CLASS*)[UIApplication sharedApplication].delegate;
				[appCon handleNotification:remoteNotificationDictionary isLaunchNotification:YES];
			});
		}

		UILocalNotification *localNotification = [note.userInfo objectForKey:UIApplicationLaunchOptionsLocalNotificationKey];
		if( localNotification )
		{
			NSLog( @"launched with local notification: %@", localNotification );
			double delayInSeconds = 5.0;
			dispatch_time_t popTime = dispatch_time( DISPATCH_TIME_NOW, (int64_t)(delayInSeconds * NSEC_PER_SEC) );
			dispatch_after( popTime, dispatch_get_main_queue(), ^
			{
				APPCONTROLLER_CLASS *appCon = (APPCONTROLLER_CLASS*)[UIApplication sharedApplication].delegate;
				[appCon handleLocalNotification:localNotification isLaunchNotification:YES];
			});
		}
	}
}


+ (void)registerForRemoteNotificationTypes:(NSNumber*)types
{
	[[UIApplication sharedApplication] registerForRemoteNotificationTypes:[types intValue]];
}


+ (NSNumber*)enabledRemoteNotificationTypes
{
	int val = [[UIApplication sharedApplication] enabledRemoteNotificationTypes];
	return [NSNumber numberWithInt:val];
}


// From: http://www.cocoadev.com/index.pl?BaseSixtyFour
- (NSString*)base64forData:(NSData*)theData
{
    const uint8_t *input = (const uint8_t*)[theData bytes];
    NSInteger length = [theData length];

    static char table[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

    NSMutableData *data = [NSMutableData dataWithLength:((length + 2) / 3) * 4];
    uint8_t *output = (uint8_t*)data.mutableBytes;

    NSInteger i;
    for( i = 0; i < length; i += 3 )
	{
        NSInteger value = 0;
        NSInteger j;
        for( j = i; j < (i + 3); j++ )
		{
            value <<= 8;

            if( j < length )
                value |= (0xFF & input[j]);
        }

        NSInteger theIndex = (i / 3) * 4;
        output[theIndex + 0] =                    table[(value >> 18) & 0x3F];
        output[theIndex + 1] =                    table[(value >> 12) & 0x3F];
        output[theIndex + 2] = (i + 1) < length ? table[(value >> 6)  & 0x3F] : '=';
        output[theIndex + 3] = (i + 2) < length ? table[(value >> 0)  & 0x3F] : '=';
    }

    return [[[NSString alloc] initWithData:data encoding:NSASCIIStringEncoding] autorelease];
}


- (void)handleLocalNotification:(UILocalNotification*)notification isLaunchNotification:(BOOL)isLaunchNotification
{
	NSMutableDictionary *dict = [NSMutableDictionary dictionaryWithObjectsAndKeys:notification.alertBody, @"alertBody",
								 notification.alertAction, @"alertAction",
								 [NSNumber numberWithInt:notification.applicationIconBadgeNumber], @"applicationIconBadgeNumber", nil];

	if( notification.alertLaunchImage )
		[dict setObject:notification.alertLaunchImage forKey:@"alertLaunchImage"];

	if( notification.userInfo )
		[dict setObject:notification.userInfo forKey:@"userInfo"];

	const char * managerMethod = isLaunchNotification ? "localNotificationWasReceivedAtLaunch" : "localNotificationWasReceived";

	NSString *json = [EtceteraManager jsonFromObject:dict];
	UnitySendMessage( "EtceteraManager", managerMethod, json.UTF8String );
}


- (void)handleNotification:(NSDictionary*)dict isLaunchNotification:(BOOL)isLaunchNotification
{
	NSDictionary *aps = [dict objectForKey:@"aps"];
	if( !aps )
		return;

	NSString *json = [EtceteraManager jsonFromObject:dict];

	const char * managerMethod = isLaunchNotification ? "remoteNotificationWasReceivedAtLaunch" : "remoteNotificationWasReceived";

	if( json )
		UnitySendMessage( "EtceteraManager", managerMethod, json.UTF8String );
	else
		UnitySendMessage( "EtceteraManager", managerMethod, "" );
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark UIApplicationDelegate

- (void)application:(UIApplication*)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken
{
	UnitySendDeviceToken( deviceToken );

	NSString *deviceTokenString = [[[[deviceToken description]
									 stringByReplacingOccurrencesOfString:@"<" withString:@""]
									stringByReplacingOccurrencesOfString:@">" withString:@""]
								   stringByReplacingOccurrencesOfString:@" " withString:@""];

	UnitySendMessage( "EtceteraManager", "remoteRegistrationDidSucceed", [deviceTokenString UTF8String] );

	// If this is a user deregistering for notifications, dont proceed past this point
	if( [[UIApplication sharedApplication] enabledRemoteNotificationTypes] == UIRemoteNotificationTypeNone )
	{
		NSLog( @"Notifications are disabled for this application. Not registering with Urban Airship" );
		return;
	}

	// Grab the Urban Airship info from the info.plist file
	NSString *appKey = [EtceteraManager sharedManager].urbanAirshipAppKey;
	NSString *appSecret = [EtceteraManager sharedManager].urbanAirshipAppSecret;
	NSString *alias = [EtceteraManager sharedManager].urbanAirshipAlias;

	if( !appKey || !appSecret )
		return;

    // Register the deviceToken with Urban Airship
    NSString *UAServer = @"https://go.urbanairship.com";
    NSString *urlString = [NSString stringWithFormat:@"%@%@%@/", UAServer, @"/api/device_tokens/", deviceTokenString];
    NSURL *url = [NSURL URLWithString:urlString];

    NSMutableURLRequest *request = [[NSMutableURLRequest alloc] initWithURL:url];
    [request setHTTPMethod:@"PUT"];

	// handle the alias if we are sending one
	if( alias )
	{
		[request setValue:@"application/json" forHTTPHeaderField:@"Content-Type"];
		NSDictionary *dict = [NSDictionary dictionaryWithObject:alias forKey:@"alias"];
		NSData *data = [[EtceteraManager jsonFromObject:dict] dataUsingEncoding:NSUTF8StringEncoding];
		[request setHTTPBody:data];
	}

    // Authenticate to the server
    [request addValue:[NSString stringWithFormat:@"Basic %@",
                       [self base64forData:[[NSString stringWithFormat:@"%@:%@",
											 appKey,
											 appSecret] dataUsingEncoding: NSUTF8StringEncoding]]] forHTTPHeaderField:@"Authorization"];

    [[NSURLConnection connectionWithRequest:request delegate:self] start];
	[request release];
}


- (void)application:(UIApplication*)application didFailToRegisterForRemoteNotificationsWithError:(NSError*)error
{
	UnitySendRemoteNotificationError( error );

	UnitySendMessage( "EtceteraManager", "remoteRegistrationDidFail", [[error localizedDescription] UTF8String] );
	NSLog( @"remoteRegistrationDidFail: %@", error );
}


- (void)application:(UIApplication*)application didReceiveRemoteNotification:(NSDictionary*)userInfo
{
	[self handleNotification:userInfo isLaunchNotification:[UIApplication sharedApplication].applicationState == UIApplicationStateInactive];
	UnitySendRemoteNotification( userInfo );
}


- (void)application:(UIApplication*)application didReceiveLocalNotification:(UILocalNotification*)notification
{
	[self handleLocalNotification:notification isLaunchNotification:[UIApplication sharedApplication].applicationState == UIApplicationStateInactive];
    UnitySendLocalNotification( notification );
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSURLConnection

- (void)connection:(NSURLConnection*)theConnection didReceiveResponse:(NSURLResponse*)response
{
	UnitySendMessage( "EtceteraManager", "urbanAirshipRegistrationDidSucceed", "" );

    NSLog( @"registered with UA: %@, %d",
		  [(NSHTTPURLResponse*)response allHeaderFields],
          [(NSHTTPURLResponse*)response statusCode] );
}


- (void)connection:(NSURLConnection*)theConnection didFailWithError:(NSError*)error
{
	UnitySendMessage( "EtceteraManager", "urbanAirshipRegistrationDidFail", [[error localizedDescription] UTF8String] );
	NSLog( @"Failed to register with UA: %@", error );
}


@end



