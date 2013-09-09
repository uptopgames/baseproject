// COPYRIGHT(c) 2011, Medium Entertainment, Inc., a Delaware corporation, which operates a service
// called PlayHaven., All Rights Reserved
//  
// NOTICE:  All information contained herein is, and remains the property of Medium Entertainment, Inc.
// and its suppliers, if any.  The intellectual and technical concepts contained herein are 
// proprietary to Medium Entertainment, Inc. and its suppliers and may be covered by U.S. and Foreign
// Patents, patents in process, and are protected by trade secret or copyright law. Dissemination of this 
// information or reproduction of this material is strictly forbidden unless prior written permission 
// is obtained from Medium Entertainment, Inc.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// 
// Contact: support@playhaven.com

#import <Foundation/Foundation.h>
#import "PlayHavenSDK.h"
@class SBJsonWriterPH;
@interface PHUnityIntegration : NSObject<PHAPIRequestDelegate, PHPublisherContentRequestDelegate>
{
	SBJsonWriterPH *_writer;
	PHPurchase *currentPurchase;
}
+(PHUnityIntegration *)sharedIntegration;

@property (nonatomic, readonly) SBJsonWriterPH *writer;
@end
