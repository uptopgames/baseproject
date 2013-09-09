//
//  VungleManager.h
//  VungleTest
//
//  Created by Mike Desaro on 6/5/12.
//  Copyright (c) 2012 prime31. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "vunglepub.h"


@interface VungleManager : NSObject <VGVunglePubDelegate>


+ (VungleManager*)sharedManager;


- (void)startWithAppId:(NSString*)appId;

- (void)startWithAppId:(NSString*)appId userDataString:(NSString*)userDataString;

- (void)startWithAppId:(NSString*)appId userData:(NSDictionary*)dict;

- (void)stop;

- (void)playModalAdShowingCloseButton:(BOOL)showCloseButton;

- (void)playIncentivizedAdWithUserTag:(NSString*)user showCloseButton:(BOOL)showCloseButton;

@end
