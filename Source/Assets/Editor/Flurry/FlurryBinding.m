//
//  FlurryBinding.m
//  FlurryTest
//
//  Created by Mike on 5/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "FlurryAds.h"
#import "Flurry.h"
#import "FlurryWallet.h"
#import "FlurryManager.h"
#import "P31SharedTools.h"



// Converts NSString to C style string by way of copy (Mono will free it)
#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

// Converts C style string to NSString as long as it isnt empty
#define GetStringParamOrNil( _x_ ) ( _x_ != NULL && strlen( _x_ ) ) ? [NSString stringWithUTF8String:_x_] : nil

void UnitySendMessage( const char * className, const char * methodName, const char * param );



void uncaughtExceptionHandler( NSException *exception )
{
    [Flurry logError:@"Uncaught Exception" message:@"Crashed" exception:exception];
}



// Starts up your Flurry session.  Call on application startup.
void _flurryStartSession( const char *apiKey )
{
	[Flurry startSession:GetStringParam( apiKey )];
	
	NSSetUncaughtExceptionHandler( &uncaughtExceptionHandler );
}


// Logs an event to Flurry.  If isTimed is true, this will be a timed event and it should be paired with a call to endTimedEvent
void _flurryLogEvent( const char * eventName, bool isTimed )
{
	if( isTimed )
		[Flurry logEvent:GetStringParam( eventName ) timed:YES];
	else
		[Flurry logEvent:GetStringParam( eventName )];
}


// Logs an event with optional key/value pairs
void _flurryLogEventWithParameters( const char * eventName, const char * params, bool isTimed )
{
	NSDictionary *dict = nil;
	NSString *json = GetStringParamOrNil( params );
	if( json )
		dict = (NSDictionary*)[P31 objectFromJsonString:json];
	
	// extract the params and stick them in a dictionary
	if( isTimed )
		[Flurry logEvent:GetStringParam( eventName ) withParameters:dict timed:YES];
	else
		[Flurry logEvent:GetStringParam( eventName ) withParameters:dict];
}


// Ends a timed event that was previously started
void _flurryEndTimedEvent( const char * eventName, const char * params )
{
	NSString *json = GetStringParamOrNil( params );
	NSDictionary *dict = nil;
	if( json )
		dict = (NSDictionary*)[P31 objectFromJsonString:json];
	
	[Flurry endTimedEvent:GetStringParam( eventName ) withParameters:dict];
}


void _flurrySetAge( int age )
{
	[Flurry setAge:age];
}


void _flurrySetGender( const char * gender )
{
	[Flurry setGender:GetStringParam( gender )];
}


void _flurrySetUserId( const char * userId )
{
	[Flurry setUserID:GetStringParam( userId )];
}


void _flurrySetSessionReportsOnCloseEnabled( bool sendSessionReportsOnClose )
{
	[Flurry setSessionReportsOnCloseEnabled:sendSessionReportsOnClose];
}


void _flurrySetSessionReportsOnPauseEnabled( bool setSessionReportsOnPauseEnabled )
{
	[Flurry setSessionReportsOnPauseEnabled:setSessionReportsOnPauseEnabled];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - Ads

void _flurryAdsInitialize( BOOL enableTestAds )
{
	[FlurryAds initialize:[FlurryManager unityViewController]];
	
	if( enableTestAds )
		[FlurryAds enableTestAds:enableTestAds];
	[FlurryAds setAdDelegate:[FlurryManager sharedManager]];
}


void _flurryAdsSetUserCookies( const char * cookies )
{
	NSDictionary *dict = (NSDictionary*)[P31 objectFromJsonString:GetStringParam( cookies )];
	[FlurryAds setUserCookies:dict];
}


void _flurryAdsClearUserCookies()
{
	[FlurryAds clearUserCookies];
}


void _flurryAdsSetKeywords( const char * keywords )
{
	NSDictionary *dict = (NSDictionary*)[P31 objectFromJsonString:GetStringParam( keywords )];
	[FlurryAds setKeywordsForTargeting:dict];
}


void _flurryAdsClearKeywords()
{
	[FlurryAds clearKeywords];
}


void _flurryFetchAdForSpace( const char * space, int adSize )
{
	[FlurryAds fetchAdForSpace:GetStringParam( space ) frame:[FlurryManager unityViewController].view.frame size:(FlurryAdSize)adSize];
}


void _flurryFetchAndDisplayAdForSpace( const char * space, int adSize )
{
	[FlurryAds fetchAndDisplayAdForSpace:GetStringParam( space ) view:[FlurryManager unityViewController].view size:(FlurryAdSize)adSize];
}


BOOL _flurryIsAdAvailableForSpace( const char * space, int adSize )
{
	return [FlurryAds adReadyForSpace:GetStringParam( space )];
}


void _flurryDisplayAdForSpace( const char * space, int adSize )
{
	[FlurryAds displayAdForSpace:GetStringParam( space ) onView:[FlurryManager unityViewController].view];
}


void _flurryRemoveAdFromSpace( const char * space )
{
	[FlurryAds removeAdFromSpace:GetStringParam( space )];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - Wallet

void _flurryAddObserverForCurrency( const char * currency )
{
	[FlurryWallet addObserver:[FlurryManager sharedManager] forCurrencyKey:GetStringParam( currency ) selector:@selector(flurryWalletUpdated:error:)];
}


void _flurryDecrementCurrency( const char * currency, float amount )
{
	NSString *currencyString = GetStringParam( currency );
	[FlurryWallet decrementWalletForCurrencyKey:currencyString
						byAmount:[NSNumber numberWithFloat:amount]
			   completionHandler:^( NSError *error )
	{
		if( error )
		{
			UnitySendMessage( "FlurryManager", "onCurrencyValueFailedToUpdate", error.localizedDescription.UTF8String );
		}
		else
		{
			NSNumber *amount = [NSNumber numberWithFloat:[FlurryWallet getLastReceivedValueForCurrencyKey:currencyString]];
			NSString *result = [NSString stringWithFormat:@"%@,%@", currencyString, amount];
			
			UnitySendMessage( "FlurryManager", "onCurrencyValueUpdated", result.UTF8String );
		}
	}];
}


/*
void _flurryIncrementCurrencyInt( const char * currency, int amount )
{
	NSString *currencyString = GetStringParam( currency );
	FlurryWallet *user = [FlurryWallet lastLoggedUser];
	
	[user incrementNumberWithKey:currencyString
						byAmount:[NSNumber numberWithInt:amount]
			   completionHandler:^( NSError *error )
	 {
		 if( error )
		 {
			 UnitySendMessage( "FlurryManager", "onCurrencyValueFailedToUpdate", error.localizedDescription.UTF8String );
		 }
		 else
		 {
			 NSNumber *amount = [NSNumber numberWithInt:[user integerForKey:currencyString]];
			 NSString *result = [NSString stringWithFormat:@"%@,%@", currencyString, amount];
			 
			 UnitySendMessage( "FlurryManager", "onCurrencyValueUpdated", result.UTF8String );
		 }
	 }];
}


void _flurryIncrementCurrencyFloat( const char * currency, float amount )
{
	NSString *currencyString = GetStringParam( currency );
	FlurryWallet *user = [FlurryWallet lastLoggedUser];
	
	[user incrementNumberWithKey:currencyString
						byAmount:[NSNumber numberWithFloat:amount]
			   completionHandler:^( NSError *error )
	 {
		 if( error )
		 {
			 UnitySendMessage( "FlurryManager", "onCurrencyValueFailedToUpdate", error.localizedDescription.UTF8String );
		 }
		 else
		 {
			 NSNumber *amount = [NSNumber numberWithFloat:[user floatForKey:currencyString]];
			 NSString *result = [NSString stringWithFormat:@"%@,%@", currencyString, amount];
			 
			 UnitySendMessage( "FlurryManager", "onCurrencyValueUpdated", result.UTF8String );
		 }
	 }];
}
*/


float _flurryGetLastReceivedCurrencyAmount( const char * currency )
{
	return [FlurryWallet getLastReceivedValueForCurrencyKey:GetStringParam( currency )];
}


