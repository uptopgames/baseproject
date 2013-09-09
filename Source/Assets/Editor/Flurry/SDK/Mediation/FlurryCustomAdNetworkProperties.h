//
//  FlurryCustomAdNetworkProperties.h
//  FlurryAds
//
//  Copyright 2010 - 2012 Flurry, Inc. All rights reserved.
//
//  Methods in this header file are for use with FlurryAds
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

/*!
 *  @brief Provides information necessary to instantiate a Custom Ad Network.
 *  
 *  Set of methods that allow publishers to provide information necessary to instatntiate and run a custom ad networks.
 *  
 *  @note This class serves as a delegate for FlurryAds. \n
 *  For additional information on how to use Flurry's Ads SDK to
 *  attract high-quality users and monetize your user base see <a href="http://wiki.flurry.com/index.php?title=Publisher">Support Center - Publisher</a>.
 *  @author 2010 - 2012 Flurry, Inc. All Rights Reserved.
 *  @version 4.0.0
 * 
 */
@protocol FlurryCustomAdNetworkProperties <NSObject>

/*!
 *  @brief Called by the custom ad network to retrieve the id for the publisher.
 *  @since 4.0.0
 * 
 *  This method provides information necessary to identify a publisher to the ad network.
 *
 *  @see FlurryCustomAdNetworkDelegate#initWithAdProperites for details on the method that will invoke this delegate.
 *
 *  @return The publisher's id on the ad network.
 */
- (NSString *) publisherId;

/*!
 *  @brief Available to the custom ad network if more information if required to id a publisher (e.g. - multiple ids). 
 *  @since 4.0.0
 * 
 *  This method may be used in place of #publisherId to pass many properties to the ad network.
 *
 *  @return A dictionary containing properties to identify a publisher.
 */
- (NSDictionary *) publisherIdExtended;

/*!
 *  @brief Informs ad network if user would like test ads delivered.
 *  @since 4.0.0
 * 
 *  This method informs the ad network this publisher is currently testing.
 *
 *  @return YES/NO to indicate if this ad request is for a test ad. 
 */
- (BOOL) testAdsEnabled;

/*!
 *  @brief Provides data supplied by the user for the purposes of targeting.
 *  @since 4.0.0
 * 
 *  This method informs the ad network of targeting data.
 *
 *  @return A dictionary of key/value pairs to be used for the purposes of targeting. Should be nil if publisher does not pass these values explicitly through this interface.
 */
- (NSDictionary *) targetData;

@optional

/*!
 *  @brief Returns the view controller to be used for displaying an interstitial.
 *  @since 4.0.0
 * 
 *  This method provides the view controller of the publisher app suited to present an interstitial.
 *
 *  @note The publisher does not need to override this method. The viewcontoller obatained from FlurryAds#initialize: will be used here.
 *  @return The version of the network sdk.
 */
- (UIViewController *) publisherViewController;
@end
