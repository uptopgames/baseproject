//
//  FlurryManager.m
//  Unity-iPhone
//
//  Created by Mike Desaro on 3/6/12.
//  Copyright (c) 2012 prime31. All rights reserved.
//

#import "FlurryManager.h"
#import "FlurryAds.h"
#import "Flurry.h"
#import "P31SharedTools.h"


void UnityPause( bool pause );
void UnitySendMessage( const char * className, const char * methodName, const char * param );
UIViewController *UnityGetGLViewController();


@implementation FlurryManager

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Class Methods

+ (FlurryManager*)sharedManager
{
	static FlurryManager *sharedSingleton;
	
	if( !sharedSingleton )
		sharedSingleton = [[FlurryManager alloc] init];
	
	return sharedSingleton;
}


+ (UIViewController*)unityViewController
{
	return UnityGetGLViewController();
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Private

- (NSString*)cacheImage:(UIImage*)image withName:(NSString*)filename
{
	static NSString *cacheDir = nil;
	
	if( !cacheDir )
	{
		NSArray *paths = NSSearchPathForDirectoriesInDomains( NSCachesDirectory, NSUserDomainMask, YES );
		cacheDir = [[paths objectAtIndex:0] retain];
	}
	
	// fix up the filename
	NSCharacterSet *charSet = [NSCharacterSet characterSetWithCharactersInString:@" ()=<>-#/"];
	filename = [[filename componentsSeparatedByCharactersInSet:charSet] componentsJoinedByString:@""];
	
	// append the .png and write to disk
	filename = [filename stringByAppendingString:@".png"];
	NSString *fullPath = [cacheDir stringByAppendingPathComponent:filename];
	NSData *rawImage = UIImagePNGRepresentation( image );
	
	[rawImage writeToFile:fullPath atomically:NO];
	
	return fullPath;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - FlurryAdDelegate

- (void)spaceDidReceiveAd:(NSString*)adSpace
{
	UnitySendMessage( "FlurryManager", "spaceDidReceiveAd", adSpace.UTF8String );
}


- (void)spaceDidFailToReceiveAd:(NSString*)adSpace error:(NSError*)error
{
	NSLog( @"space failed to receive ad with error: %@", error );
	UnitySendMessage( "FlurryManager", "spaceDidFailToReceiveAd", adSpace.UTF8String );
}


- (BOOL)spaceShouldDisplay:(NSString*)adSpace interstitial:(BOOL)interstitial
{
	if( interstitial )
		UnityPause( true );
	
	return YES;
}


- (void)spaceWillDismiss:(NSString*)adSpace interstitial:(BOOL)interstitial
{
	if( interstitial )
		UnityPause( false );
}


- (void)spaceDidDismiss:(NSString*)adSpace interstitial:(BOOL)interstitial
{
	if( interstitial )
		UnityPause( false );
	
	UnitySendMessage( "FlurryManager", "spaceDidDismiss", adSpace.UTF8String );
}


- (void)spaceWillLeaveApplication:(NSString*)adSpace
{
	UnitySendMessage( "FlurryManager", "spaceWillLeaveApplication", adSpace.UTF8String );
}


- (void)spaceDidFailToRender:(NSString*)space
{
	UnitySendMessage( "FlurryManager", "spaceDidFailToRender", space.UTF8String );
}


- (void)spaceWillExpand:(NSString*)adSpace
{
	UnityPause( true );
}


- (void)spaceWillCollapse:(NSString*)adSpace
{
	UnityPause( false );
}


- (void)spaceDidReceiveClick:(NSString*)adSpace
{
	NSLog( @"%@", NSStringFromSelector( _cmd ) );
}


- (void)videoDidFinish:(NSString*)adSpace
{
	UnitySendMessage( "FlurryManager", "videoDidFinish", adSpace.UTF8String );
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - FlurryWallet Listener

- (void)flurryWalletUpdated:(FlurryWalletInfo*)walletInfo error:(NSError*)error
{
	if( error )
	{
		UnitySendMessage( "FlurryManager", "onCurrencyValueFailedToUpdate", [P31 jsonFromError:error] );
	}
	else
	{
		NSNumber *amount = [walletInfo currencyAmount];
		NSString *currency = [walletInfo currencyKey];
		NSString *result = [NSString stringWithFormat:@"%@,%@", currency, amount];
		
		UnitySendMessage( "FlurryManager", "onCurrencyValueUpdated", result.UTF8String );
	}
}


@end
