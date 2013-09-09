//
//  FlurryCustomAdNetworkDelegate.h
//  FlurryAds
//
//  Copyright 2010 - 2012 Flurry, Inc. All rights reserved.
//
//  Methods in this header file are for use with FlurryAds
//

#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>

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
@protocol FlurryCustomAdNetworkDelegate <NSObject>

#pragma mark -
#pragma mark Network Callbacks

/*!
 *  @brief This call must be invoked when network fails to receive an ad.
 *  @since 4.0.0
 * 
 *  This method informs the mediation layer an ad could not be retrieved.
 *
 *  @param error The error returned from the network SDK if available. If not available just set to nil.
 *  @param interstitialAd YES/NO to indicate if the ad failure was for an interstitial ad.
 */
- (void) networkDidFailAd:(NSError *)error interstitialAd:(BOOL)interstitialAd;

/*!
 *  @brief This call should be invoked when the network received an ad.
 *  @since 4.0.0
 * 
 *  This method informs mediation layer an ad was retrieved successfully.
 *
 *  @param adView The ad to be used for this request. If an interstital ad was received and a reference is not available, just set this to nil. If an inline ad was requested and the adView is nil, this is an error and not result in a view attempt.
 *  @param interstitialAd YES/NO to indicate if the ad received was for an interstitial ad.
 */
- (void) networkDidReceiveAd:(UIView *)adView interstitialAd:(BOOL)interstitialAd;

/*!
 *  @brief This call should be invoked when the network ad receives a click.
 *  @since 4.0.0
 * 
 *  This method informs mediation layer an ad was clicked.
 *
 *  @note This method should be called for any interaction that should be noted as a click by the user. A click MUST be explicitly registered with this callback. Other methods such as #networkWillExpandAd will not register a click.
 *
 *  @param interstitialAd YES/NO to indicate if the ad clicked was an interstitial.
 *
 */
- (void) networkAdDidReceiveClick:(BOOL)interstitialAd;

/*!
 *  @brief This call should be invoked when the network will display an interstitial.
 *  @since 4.0.0
 * 
 *  This method informs mediation layer an interstitial will display. This will be forwarded to the publisher so they can take necessary actions to pause their app state.
 *
 */
- (void) networkWillPresentInterstitial;

/*!
 *  @brief This call should be invoked when the network will dismiss an interstitial.
 *  @since 4.0.0
 * 
 *  This method informs mediation layer the interstitial will close.
 *
 */
- (void) networkWillDismissInterstitial;

/*!
 *  @brief This call should be invoked when the network did dismiss an interstitial ad.
 *  @since 4.0.0
 * 
 *  This method informs mediation layer an interstitial was dismissed.
 *
 */
- (void) networkDidDismissInterstitial;

/*!
 *  @brief This call should be invoked when the network will send the user out of the app based on a user interaction with the ad.
 *  @since 4.0.0
 * 
 *  This method informs mediation layer the network will send the user out of the app based on a user interaction.
 *
 *  @param interstitialAd YES/NO to indicate if the original ad which led the user out of the app was an interstitial.
 *
 */
- (void) networkWillLeaveApplication:(BOOL)interstitialAd;

/*!
 *  @brief This call should be invoked when the network will expand an inline ad (typically a banner).
 *  @since 4.0.0
 * 
 *  This method informs mediation layer the network will is expanding the adView. Publishers are advised to pause their app states when this occurs
 *
 */
- (void) networkWillExpandAd;

/*!
 *  @brief This call should be invoked when the network will collapse an inline ad back to its original state (typically a banner).
 *  @since 4.0.0
 * 
 *  This method informs mediation layer the network will collapse the adView.
 *
 */
- (void) networkWillCollapseAd;

/*!
 *  @brief This call should be invoked when the network has collapsed an inline ad back to its original state (typically a banner).
 *  @since 4.0.0
 * 
 *  This method informs mediation layer the network did collapse the adView.
 *
 */
- (void) networkDidCollapseAd;

@end
