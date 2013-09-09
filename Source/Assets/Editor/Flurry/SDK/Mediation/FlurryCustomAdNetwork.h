//
//  FlurryCustomAdNetwork.h
//  FlurryAds
//
//  Copyright 2010 - 2012 Flurry, Inc. All rights reserved.
//
//  Methods in this header file are for use with FlurryAds
//

#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>
#import "FlurryCustomAdNetworkProperties.h"
#import "FlurryCustomAdNetworkDelegate.h"

/*!
 *  @brief Provides all available delegates for related to Ad Serving of a Custom Ad Network.
 *  
 *  Set of methods that allow developers to manage and take actions within
 *  different phases of App ad display for a custom network.
 *  
 *  @note This class serves as a delegate for FlurryAds. \n
 *  For additional information on how to use Flurry's Ads SDK to
 *  attract high-quality users and monetize your user base see <a href="http://wiki.flurry.com/index.php?title=Publisher">Support Center - Publisher</a>.
 *  @author 2010 - 2012 Flurry, Inc. All Rights Reserved.
 *  @version 4.0.0
 * 
 */
@protocol FlurryCustomAdNetwork <NSObject>

/*!
 *  @brief Invoked when an ad network is registered.
 *  @since 4.0.0
 * 
 *  This method allows you to intialize the ad network. For instance, you can start pre-caching of ads here.
 *
 *  @see FlurryAds#addCustomAdNetwork:withProperties: for details on the method that will invoke this delegate.
 *
 *  @param networkProperties These values are supplied by the publisher and mediation sdk to instantiate network and supply needed values for ad display.
 *
 */
- (id) initWithAdProperites:(id<FlurryCustomAdNetworkProperties>)networkProperties;

/*!
 *  @brief Invoked during initialization to set callback delegate.
 *  @since 4.0.0
 * 
 *  This method provides the object to callback. It will be set when custom network is added.
 *
 *  @see FlurryAds#addCustomAdNetwork:withProperties: for details on the method that will invoke this delegate.
 *
 *  @param mediationDelegate This is the class that should be invoked for network related callbacks. Do not retain this class.
 *
 */
- (void) setMediationDelegate:(id<FlurryCustomAdNetworkDelegate>)mediationDelegate;

/*!
 *  @brief Returns the name of the ad network. 
 *  @since 4.0.0
 * 
 *  This method identifies the ad network. This must match the ad network id as entered on the server.
 *
 *  @return The name of the network
 */
- (NSString *) adNetworkName;

/*!
 *  @brief Returns the version of the ad network.
 *  @since 4.0.0
 * 
 *  This method identifies the version of the ad network.
 *
 *  @return The version of the network sdk.
 */
- (NSString *) adNetworkVersion;

/*!
 *  @brief Requests an ad network to load an ad.
 *  @since 4.0.0
 * 
 *  This method informs the network an ad is being requested.
 *
 *  @param adSpace The placement of an ad in the publisher app, where placement may
 *  be splash screen for SPLASH_AD.
 *  @param viewFrame The representative view freame the ad will be placed within. You should use the properties of this view to determine the size of an inline ad.
 *  @param loadInterstitial YES/NO. This indicates if the ad request is to load an interstital. If this is YES, the network sdk is expected to ignore the viewContainer and prepare an interstitial.
 *
 *  @see FlurryAds#publisherViewController for details on the method that will retrieve the app's viewcontroller to display an interstitial.
 *
 *  @return YES/NO to indicate if this ad request should continue. For instance, you can return NO immediately if the ad network. Most networks will make asynchronous requests. In this case return yes and implement the adNetworkDidFailToRender delegate if an ad is not available. 
 */
- (BOOL) getAd:(NSString *)adSpace withFrame:(CGRect)viewFrame loadInterstitial:(BOOL)loadInterstitial;

/*!
 *  @brief This informs the sdk that an ad will be presented to the user. This method will record an ad impression. 
 *  @since 4.0.0
 * 
 *  This method will only be invoked after the sdk has received the #networkDidReceiveAd: callback prior to displaying the ad to the user. If showInterstital is YES, the sdk must present the interstital. If this is for an inline ad, the sdk must place the ad as a subview in the viewContainer. This viewContainer is guaranteed to have the same dimensions as specified in #getAd:dimensions:loadInterstitial. 
 *   If the network can not Immediately show an ad, they must return NO. If YES is returned, but the network still fails to render an ad, the network sdk must call #networkDidFailAd:. 
 *
 *  @param viewContainer This is the view the add should be placed into.
 *  @param showInterstitial YES/NO Indicates if the ad display is for an interstitial.
 *
 *  @see FlurryAds#publisherViewController for details on the method that will retrieve the app's viewcontroller to display an interstitial.
 */
- (BOOL) adWillDisplay:(UIView *)viewContainer showInterstitial:(BOOL)showInterstitial;

/*!
 *  @brief Notifies to network sdk this ad request has completed. Any delegate corresponding to this ad request should be set to nil here.
 *  @since 4.0.0
 * 
 *  This method informs the network sdk the ad request has completed. It may commence any cleanup based on the single request.
 *
 *  @note This method may be called multiple times during the lifecycle of an app and must have no side effects from repeated invocations.
 *  
 *  @param interstitial YES/NO If the ad request completed was an interstitial.
 */
- (void) adRequestComplete:(BOOL)interstitial;

@end
