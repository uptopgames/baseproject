//
//  AdManager.m
//  iAd
//
//  Created by Mike on 8/18/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import "AdManager.h"


UIViewController *UnityGetGLViewController();

void UnityPause( bool pause );

void UnitySendMessage( const char * className, const char * methodName, const char * param );


@interface AdManager(Private)
- (void)adjustRequestedAdTypesBasedOnOrientation;
- (void)adjustAdViewFrameToShowAdView;
@end



@implementation AdManager

@synthesize adView = _adView, fireHideShowEvents = _fireHideShowEvents, interstitial = _interstitial, adBannerOnBottom = _adBannerOnBottom, isShowingBanner;

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

+ (AdManager*)sharedManager
{
	static AdManager *sharedSingleton;
	
	if( !sharedSingleton )
		sharedSingleton = [[AdManager alloc] init];
	
	return sharedSingleton;
}


- (id)init
{
	// early out if we dont have iOS 4.0 iAd.framework
	if( !NSClassFromString( @"ADBannerView" ) )
		return nil;
	
	if( self = [super init] )
	{
		// sensible defaults
		_adBannerOnBottom = YES;
		
		// grab our orientation
		_orientation = [UIApplication sharedApplication].statusBarOrientation;
		
		[[NSNotificationCenter defaultCenter] addObserver:self
												 selector:@selector(orientationChanged:)
													 name:UIApplicationWillChangeStatusBarOrientationNotification
												   object:nil];
		[[NSNotificationCenter defaultCenter] addObserver:self
												 selector:@selector(orientationChanged:)
													 name:UIApplicationDidChangeStatusBarOrientationNotification
												   object:nil];
	}
	return self;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - NSNotification

- (void)orientationChanged:(NSNotification*)note
{
	if( _ignoreOrientationNotifications || !isShowingBanner )
		return;
	
	static BOOL _bannerNeedsRecreation = NO;
	
	// in will change we get the new orientation. in did change we recreate the banner
	if( [note.name isEqualToString:UIApplicationWillChangeStatusBarOrientationNotification] )
	{
		BOOL isLandscape = UIInterfaceOrientationIsLandscape( _orientation );
		
		NSNumber *num = [note.userInfo objectForKey:UIApplicationStatusBarOrientationUserInfoKey];
		_orientation = (UIInterfaceOrientation)[num intValue];
		
		// did we switch from a landscape to a portrait orientation?
		if( UIInterfaceOrientationIsLandscape( _orientation ) != isLandscape )
		{
			[self destroyAdBanner];
			_bannerNeedsRecreation = YES;
		}
	}
	else
	{
		if( _bannerNeedsRecreation )
		{
			_bannerNeedsRecreation = NO;
			[self createAdBanner];
		}
	}
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Private

- (void)adjustRequestedAdTypesBasedOnOrientation
{
	if( !_adView )
		return;
	
	// set the contentSize and requiredContentSize so the adView knows what it can display
	if( UIInterfaceOrientationIsLandscape( _orientation ) )
	{
		// use the new add banner content size identifiers when available
		if( &ADBannerContentSizeIdentifierLandscape != NULL )
		{
			_adView.requiredContentSizeIdentifiers = [NSSet setWithObject:ADBannerContentSizeIdentifierLandscape];
			_adView.currentContentSizeIdentifier = ADBannerContentSizeIdentifierLandscape;
		}
		else
		{
			_adView.requiredContentSizeIdentifiers = [NSSet setWithObject:ADBannerContentSizeIdentifier480x32];
			_adView.currentContentSizeIdentifier = ADBannerContentSizeIdentifier480x32;
		}
	}
	else
	{
		// use the new add banner content size identifiers when available
		if( &ADBannerContentSizeIdentifierPortrait != NULL )
		{
			_adView.requiredContentSizeIdentifiers = [NSSet setWithObject:ADBannerContentSizeIdentifierPortrait];
			_adView.currentContentSizeIdentifier = ADBannerContentSizeIdentifierPortrait;
		}
		else
		{
			_adView.requiredContentSizeIdentifiers = [NSSet setWithObject:ADBannerContentSizeIdentifier320x50];
			_adView.currentContentSizeIdentifier = ADBannerContentSizeIdentifier320x50;
		}
	}
	
	[self adjustAdViewFrameToShowAdView];
}


- (void)adjustAdViewFrameToShowAdView
{
	CGRect origFrame = _adView.frame;
	if( _adBannerOnBottom )
	{
		CGFloat screenHeight = [UIScreen mainScreen].bounds.size.height;
		if( UIInterfaceOrientationIsLandscape( _orientation ) )
			screenHeight = [UIScreen mainScreen].bounds.size.width;
		
		origFrame.origin = CGPointMake( 0, screenHeight - origFrame.size.height );
	}
	else
	{
		origFrame.origin = CGPointMake( 0, 0 );
	}
	_adView.frame = origFrame;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Public

- (void)createAdBanner
{
	// if we have an adView dont create one
	if( _adView )
		return;

	_adView = [[ADBannerView alloc] initWithFrame:CGRectZero];
	
	[self adjustRequestedAdTypesBasedOnOrientation];
	_adView.delegate = self;
	
	[UnityGetGLViewController().view addSubview:_adView];
	[self adjustAdViewFrameToShowAdView];
	
	// hide the banner if there is no ad loaded
	if( !_adView.bannerLoaded )
		_adView.hidden = YES;
}


- (void)destroyAdBanner
{
	// destroy the adView
	_adView.delegate = nil;
	[_adView removeFromSuperview];
	self.adView = nil;
	
	_bannerIsVisible = NO;
}


- (void)setBannerIsOnBottom:(BOOL)isBottom
{
	_adBannerOnBottom = isBottom;
	
	if( _adView )
		[self adjustAdViewFrameToShowAdView];
}


// interstitial methods
- (BOOL)initializeInterstitial
{
	// iPad only
	if( UI_USER_INTERFACE_IDIOM() != UIUserInterfaceIdiomPad )
		return NO;
	
	// early out if we dont have iOS 4.3 iAd.framework
	if( !NSClassFromString( @"ADInterstitialAd" ) )
		return NO;
	
	self.interstitial = [[[ADInterstitialAd alloc] init] autorelease];
	_interstitial.delegate = self;
	
	return YES;
}


- (BOOL)interstitialIsLoaded
{
	if( _interstitial )
		return _interstitial.isLoaded;
	return NO;
}


- (BOOL)showInterstitial
{
	if( !_interstitial.isLoaded )
		return NO;
	
	UnityPause( true );
	
	if( _adView )
	{
		NSLog( @"ad banner being destroyed because you cannot display a banner and an interstitial at the same time" );
		[self destroyAdBanner];
	}

	// show the ad
	[_interstitial presentFromViewController:UnityGetGLViewController()];
	
	return YES;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark ADBannerViewDelegate

- (void)bannerView:(ADBannerView*)banner didFailToReceiveAdWithError:(NSError*)error
{
	NSLog( @"------ bannerView:didFailToReceiveAdWithError: %@", [error localizedDescription] );
	_adView.hidden = YES;
	
	_bannerIsVisible = NO;
	
	// fire the event if we want it
	if( _fireHideShowEvents )
		UnitySendMessage( "AdManager", "adViewDidShow", "0" );
}


- (void)bannerViewDidLoadAd:(ADBannerView*)banner
{
	_adView.hidden = NO;
	
    if( !_bannerIsVisible )
    {
		_bannerIsVisible = YES;
		
		// fire the event if we want it.  we only fire this event when we change from not visible to visible
		if( _fireHideShowEvents )
			UnitySendMessage( "AdManager", "adViewDidShow", "1" );
    }
}


- (BOOL)bannerViewActionShouldBegin:(ADBannerView*)banner willLeaveApplication:(BOOL)willLeave
{
	NSLog( @"bannerViewActionShouldBegin:willLeaveApplication:(%@)", willLeave ? @"YES" : @"NO" );
	
	if( !willLeave )
	{
		_ignoreOrientationNotifications = YES;
		UnityPause( true );
	}
	
	return YES;
}


- (void)bannerViewActionDidFinish:(ADBannerView*)banner
{
	NSLog( @"bannerViewActionDidFinish:" );
	[self adjustAdViewFrameToShowAdView];
	_ignoreOrientationNotifications = NO;
	
	UnityPause( false );
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark ADInterstitialAdDelegate

- (void)interstitialAdDidUnload:(ADInterstitialAd*)interstitialAd
{
	// release the interstitial and init a new one
	_interstitial.delegate = nil;
	self.interstitial = nil;
	
	UnityPause( false );
}


- (void)interstitialAdActionDidFinish:(ADInterstitialAd*)interstitialAd
{
	UnityPause( false );
}


- (void)interstitialAd:(ADInterstitialAd*)interstitialAd didFailWithError:(NSError*)error
{
	UnitySendMessage( "AdManager", "interstitialFailed", [[error localizedDescription] UTF8String] );
	
	_interstitial.delegate = nil;
	self.interstitial = nil;
}


- (void)interstitialAdDidLoad:(ADInterstitialAd*)interstitialAd
{
	UnitySendMessage( "AdManager", "interstitialLoaded", "" );
}


@end
