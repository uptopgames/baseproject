//
//  AppControllerPushAdditions.h
//  EtceteraTest
//
//  Created by Mike on 10/5/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import <Foundation/Foundation.h>

#if UNITY_VERSION < 420

#import "AppController.h"
#define APPCONTROLLER_CLASS AppController
@interface AppController(PushAdditions)

#else

#import "UnityAppController.h"
#define APPCONTROLLER_CLASS UnityAppController
@interface UnityAppController(PushAdditions)

#endif




+ (void)registerForRemoteNotificationTypes:(NSNumber*)types;

+ (NSNumber*)enabledRemoteNotificationTypes;

@end
