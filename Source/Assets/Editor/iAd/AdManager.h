//
//  AdManager.h
//  iAd
//
//  Created by Mike on 8/18/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <iAd/iAd.h>


@interface AdManager : NSObject <ADBannerViewDelegate, ADInterstitialAdDelegate>
{
@private
	UIInterfaceOrientation _orientation;
	BOOL _adBannerOnBottom;
	BOOL _bannerIsVisible;
	BOOL _ignoreOrientationNotifications;
}
@property (nonatomic, assign) BOOL isShowingBanner;
@property (nonatomic, assign) BOOL adBannerOnBottom;
@property (nonatomic, retain) ADBannerView *adView;
@property (nonatomic, assign) BOOL fireHideShowEvents;
@property (nonatomic, retain) ADInterstitialAd *interstitial;


+ (AdManager*)sharedManager;

- (void)createAdBanner;

- (void)destroyAdBanner;

- (void)setBannerIsOnBottom:(BOOL)isBottom;

- (BOOL)initializeInterstitial;

- (BOOL)interstitialIsLoaded;

- (BOOL)showInterstitial;

@end
