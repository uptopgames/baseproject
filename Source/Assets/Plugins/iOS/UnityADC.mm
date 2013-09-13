//=============================================================================
//  UnityADC.mm
//
//  iOS functionality for the Unity AdColony plug-in.
//
//  Copyright 2010 Jirbo, Inc.  All rights reserved.
//
//  ---------------------------------------------------------------------------
//
//  * Instructions *
//
//  Copy this file into your Unity project's Assets/Plugins/iOS folder.
//
//  Refer to the header comment in AdColony.cs for further instructions.
//
//=============================================================================

#import "AdColonyPublic.h"

void UnityPause(bool pause);


@interface UnityADCIOSDelegate : NSObject<AdColonyAdministratorDelegate,AdColonyTakeoverAdDelegate>
{
}
- (NSString*) adColonyApplicationID;
- (NSString*) adColonyApplicationVersion;
- (NSDictionary*) adColonyAdZoneNumberAssociation;
- (void) adColonyTakeoverBeganForZone:(NSString *)zone;
- (void) adColonyTakeoverEndedForZone:(NSString*)zone withVC:(BOOL)vc;
- (void) adColonyVideoAdNotServedForZone:(NSString*)zone;
- (NSString*) adColonyLoggingStatus;
@end

NSString*     adc_app_version = nil;
NSString*     adc_app_id = nil;
NSString*     adc_cur_zone = nil;
NSMutableDictionary* adc_zone_ids = nil;
UnityADCIOSDelegate* adc_ios_delegate = nil;

NSString* set_adc_cur_zone( NSString* new_adc_cur_zone )
{
  if (adc_cur_zone) [adc_cur_zone release];
  adc_cur_zone = [new_adc_cur_zone retain];
  return adc_cur_zone;
}


@implementation UnityADCIOSDelegate
- (NSString *) adColonyApplicationID
{
    return adc_app_id;
}

- (NSString*) adColonyApplicationVersion
{
    return adc_app_version;
}

-(NSDictionary *)adColonyAdZoneNumberAssociation
{
    return adc_zone_ids;
}

- (void) adColonyTakeoverBeganForZone:(NSString *)zone
{
    UnitySendMessage( "AdColony", "OnAdColonyVideoStarted", "" );
}

- (void) adColonyTakeoverEndedForZone:(NSString*)zone withVC:(BOOL)vc
{
    UnitySendMessage( "AdColony", "OnAdColonyVideoFinished", "" );
}

- (void) adColonyVideoAdNotServedForZone:(NSString*)zone
{
    UnitySendMessage( "AdColony", "OnAdColonyVideoFinished", "" );
}


- (void) adColonyVirtualCurrencyAwardedByZone:(NSString *)zone
                                 currencyName:(NSString *)name currencyAmount:(int)amount
{
    UnitySendMessage( "AdColony", "OnAdColonyV4VCResult",
                     [[NSString stringWithFormat:@"true|%d|%@", amount, name] UTF8String] );
}

- (void) adColonyVirtualCurrencyNotAwardedByZone:(NSString *)zone
                                    currencyName:(NSString *)name currencyAmount:(int)amount reason:(NSString *)reason
{
    UnitySendMessage( "AdColony", "OnAdColonyV4VCResult",
                     [[NSString stringWithFormat:@"false|%d|%@", amount, name] UTF8String] );
}

- (NSString*) adColonyLoggingStatus
{
    return AdColonyLoggingOn;
}

@end

#include <iostream>
using namespace std;

extern "C"
{
    void  IOSConfigure( const char* app_version, const char* app_id, int zone_id_count, const char* zone_ids[] )
    {
        adc_app_version = [[NSString stringWithUTF8String:app_version] retain];
        adc_app_id = [[NSString stringWithUTF8String:app_id] retain];

        adc_zone_ids = [[NSMutableDictionary dictionary] retain];
        for (int i=0; i<zone_id_count; ++i)
        {
            NSString* zone_id_str = [NSString stringWithUTF8String:zone_ids[i]];
            [adc_zone_ids setObject:zone_id_str forKey:[NSNumber numberWithInt:i+1]];
            if (i == 0) set_adc_cur_zone( zone_id_str );
        }

        adc_ios_delegate = [[[UnityADCIOSDelegate alloc] init] retain];
        [AdColonyAdministratorPublic initAdministratorWithDelegate:adc_ios_delegate];
    }

    bool  IOSIsVideoAvailable( const char* zone_id )
    {
        NSString* zid = adc_cur_zone;
        if (zone_id && zone_id[0] != 0)
        {
            zid = set_adc_cur_zone( [NSString stringWithUTF8String:zone_id] );
        }
        return [AdColonyAdministratorPublic didVideoFinishLoadingForZone:zid];
    }

    bool  IOSIsV4VCAvailable( const char* zone_id )
    {
        NSString* zid = adc_cur_zone;
        if (zone_id && zone_id[0] != 0)
        {
            zid = set_adc_cur_zone( [NSString stringWithUTF8String:zone_id] );
        }
        if ( !IOSIsVideoAvailable(zone_id) ) return false;
        return [AdColonyAdministratorPublic virtualCurrencyAwardAvailableForZone:zid];
    }

    char* IOSGetDeviceID()
    {
        NSString* result_str = [AdColony getUniqueDeviceID];
        if (result_str)
        {
            const char *c_str = [result_str UTF8String];
            int count = strlen( c_str );
            char* result = (char *)malloc(count + 1);
            strcpy( result, c_str );
            return result;
        }
        else
        {
            char* result = new char[10];
            strcpy( result, "undefined" );
            return result;
        }
    }

    char* IOSGetOpenUDID()
    {
        NSString* result_str = [AdColony getOpenUDID];
        if (result_str)
        {
            const char *c_str = [result_str UTF8String];
            int count = strlen( c_str );
            char* result = (char *)malloc(count + 1);
            strcpy( result, c_str );
            return result;
        }
        else
        {
            char* result = new char[10];
            strcpy( result, "undefined" );
            return result;
        }
    }

    char* IOSGetODIN1()
    {
        NSString* result_str = [AdColony getODIN1];
        if (result_str)
        {
            const char *c_str = [result_str UTF8String];
            int count = strlen( c_str );
            char* result = (char *)malloc(count + 1);
            strcpy( result, c_str );
            return result;
        }
        else
        {
            char* result = new char[10];
            strcpy( result, "undefined" );
            return result;
        }
    }

    int   IOSGetV4VCAmount( const char* zone_id )
    {
        NSString* zid = adc_cur_zone;
        if (zone_id && zone_id[0] != 0)
        {
            zid = set_adc_cur_zone( [NSString stringWithUTF8String:zone_id] );
        }
        return [AdColonyAdministratorPublic getVirtualCurrencyRewardAmountForZone:zid];
    }

    char* IOSGetV4VCName( const char* zone_id )
    {
        NSString* zid = adc_cur_zone;
        if (zone_id && zone_id[0] != 0)
        {
            zid = set_adc_cur_zone( [NSString stringWithUTF8String:zone_id] );
        }
        NSString* result_str = [AdColonyAdministratorPublic getVirtualCurrencyNameForZone:zid];
        if (result_str)
        {
            const char *c_str = [result_str UTF8String];
            int count = strlen( c_str );
            char* result = (char *)malloc(count + 1);
            strcpy( result, c_str );
            return result;
        }
        else
        {
            char* result = new char[10];
            strcpy( result, "undefined" );
            return result;
        }
    }

    bool  IOSShowVideoAd( const char* zone_id )
    {
        NSString* zid = adc_cur_zone;
        if (zone_id && zone_id[0] != 0)
        {
            zid = set_adc_cur_zone( [NSString stringWithUTF8String:zone_id] );
        }
        if ( !IOSIsVideoAvailable(zone_id) ) return false;

        [AdColonyAdministratorPublic playVideoAdForZone:zid withDelegate:adc_ios_delegate
                                       withV4VCPrePopup:NO andV4VCPostPopup:NO];
        return true;
    }

    bool  IOSShowV4VC( bool popup_result, const char* zone_id )
    {
        NSString* zid = adc_cur_zone;
        if (zone_id && zone_id[0] != 0)
        {
            zid = set_adc_cur_zone( [NSString stringWithUTF8String:zone_id] );
        }
        if ( !IOSIsV4VCAvailable(zone_id) ) return false;

        [AdColonyAdministratorPublic playVideoAdForZone:zid withDelegate:adc_ios_delegate
                                       withV4VCPrePopup:NO andV4VCPostPopup:popup_result];
        return true;
    }

    void  IOSOfferV4VC( bool popup_result, const char* zone_id )
    {
        NSString* zid = adc_cur_zone;
        if (zone_id && zone_id[0] != 0)
        {
          zid = set_adc_cur_zone( [NSString stringWithUTF8String:zone_id] );
        }
        if ( !IOSIsV4VCAvailable(zone_id) ) return;

        [AdColonyAdministratorPublic playVideoAdForZone:zid withDelegate:adc_ios_delegate
                                       withV4VCPrePopup:YES andV4VCPostPopup:popup_result];
    }

}
