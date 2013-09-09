//
//  FlurryManager.h
//  Unity-iPhone
//
//  Created by Mike Desaro on 3/6/12.
//  Copyright (c) 2012 prime31. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "FlurryAdDelegate.h"
#import "FlurryWallet.h"


@interface FlurryManager : NSObject <FlurryAdDelegate>


+ (FlurryManager*)sharedManager;

+ (UIViewController*)unityViewController;


- (void)flurryWalletUpdated:(FlurryWalletInfo*)walletInfo error:(NSError*)error;


@end
