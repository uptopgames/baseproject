//
//  AdBinding.m
//  iAd
//
//  Created by Mike on 8/18/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import "AdManager.h"


void _iAdCreateAdBanner( bool bannerOnBottom )
{
	[AdManager sharedManager].isShowingBanner = YES;
	[AdManager sharedManager].adBannerOnBottom = bannerOnBottom;
	[[AdManager sharedManager] createAdBanner];
}


void _iAdDestroyAdBanner()
{
	[[AdManager sharedManager] destroyAdBanner];
	[AdManager sharedManager].isShowingBanner = NO;
}


void _iAdFireHideShowEvents( bool shouldFire )
{
	[AdManager sharedManager].fireHideShowEvents = shouldFire;
}


bool _iAdInitializeInterstitial()
{
	return [[AdManager sharedManager] initializeInterstitial];
}


bool _iAdInterstitialIsLoaded()
{
	return [[AdManager sharedManager] interstitialIsLoaded];
}


bool _iAdShowInterstitial()
{
	return [[AdManager sharedManager] showInterstitial];
}

