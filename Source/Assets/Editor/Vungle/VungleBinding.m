//
//  VungleBinding.m
//  VungleTest
//
//  Created by Mike Desaro on 6/5/12.
//  Copyright (c) 2012 prime31. All rights reserved.
//
#import "vunglepub.h"
#import "VungleManager.h"


// Converts NSString to C style string by way of copy (Mono will free it)
#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

// Converts C style string to NSString as long as it isnt empty
#define GetStringParamOrNil( _x_ ) ( _x_ != NULL && strlen( _x_ ) ) ? [NSString stringWithUTF8String:_x_] : nil




void _vungleStartWithAppId( const char * appId )
{
	[[VungleManager sharedManager] startWithAppId:GetStringParam( appId )];
}


void _vungleSetSoundEnabled( BOOL enabled )
{
	[VGVunglePub setSoundEnabled:enabled];
}


void _vungleStartWithAppIdAndUserData( const char * appId, const char * userData )
{
	[[VungleManager sharedManager] startWithAppId:GetStringParam( appId ) userDataString:GetStringParam( userData )];
}


void _vungleEnableLogging( BOOL shouldEnable )
{
	[VGVunglePub logToStdout:shouldEnable];
}


void _vungleSetCacheSize( int cacheSize )
{
	[VGVunglePub cacheSizeSet:cacheSize];
}


BOOL _vungleIsAdAvailable()
{
	return [VGVunglePub adIsAvailable];
}


void _vungleStop()
{
	[VGVunglePub stop];
}


void _vunglePlayModalAd( BOOL showCloseButton )
{
	[[VungleManager sharedManager] playModalAdShowingCloseButton:showCloseButton];
}


void _vunglePlayInsentivisedAd( const char * user, BOOL showCloseButton )
{
	[[VungleManager sharedManager] playIncentivizedAdWithUserTag:GetStringParam( user ) showCloseButton:showCloseButton];
}


void _vungleAllowAutoRotate( bool shouldAllow )
{
	[VGVunglePub allowAutoRotate:shouldAllow];
}