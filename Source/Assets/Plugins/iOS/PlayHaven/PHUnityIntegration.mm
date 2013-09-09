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

#import "PHUnityIntegration.h"
#import "PHPublisherMetadataRequest.h"
#import "PHAPIRequest.h"
#import "JSON.h"

#define UNITY_SDK_VERSION @"ios-unity-1.14.2"

//#define OPENUDID_SUPPORT

#pragma mark - Unity Externs
extern void UnitySendMessage(const char *obj, const char *method, const char *msg);

#pragma mark -

static PHUnityIntegration *sharedIntegration;

@interface PHUnityIntegration()
+(void)cancelRequestWithHashCode:(int)hashCode;
-(void)productPurchaseResolution:(int)action;
@end

@implementation PHUnityIntegration

+(PHUnityIntegration *)sharedIntegration
{
    if (sharedIntegration == nil)
    {
        sharedIntegration = [PHUnityIntegration new];
        [PHAPIRequest setPluginIdentifier:UNITY_SDK_VERSION];
    }
    
    return sharedIntegration;
}

-(SBJsonWriterPH *)writer
{
  if (_writer == nil) {
    _writer = [SBJsonWriterPH new];
  }
  
  return _writer;
}

-(void)request:(PHAPIRequest *)request didSucceedWithResponse:(NSDictionary *)responseData
{
    NSDictionary *messageDictionary = [NSDictionary dictionaryWithObjectsAndKeys:
                                       [NSNumber numberWithInt:request.hashCode],@"hash",
                                       @"success", @"name",
                                       (!!responseData)? responseData: [NSDictionary dictionary],@"data", 
                                       nil];
    NSString *messageJSON = [self.writer stringWithObject:messageDictionary];
    UnitySendMessage("PlayHavenManager", "HandleNativeEvent", [messageJSON cStringUsingEncoding:NSUTF8StringEncoding]);
}

-(void)request:(PHAPIRequest *)request didFailWithError:(NSError *)error
{
    NSDictionary *errorData = [NSDictionary dictionaryWithObjectsAndKeys:
                               [NSNumber numberWithInt:error.code],@"code",
                               error.localizedDescription,@"description",
                               nil];
    NSDictionary *messageDictionary = [NSDictionary dictionaryWithObjectsAndKeys:
                                       [NSNumber numberWithInt:request.hashCode],@"hash",
                                       @"error", @"name",
                                       errorData,@"data", 
                                       nil];
    NSString *messageJSON = [self.writer stringWithObject:messageDictionary];
    UnitySendMessage("PlayHavenManager", "HandleNativeEvent", [messageJSON cStringUsingEncoding:NSUTF8StringEncoding]);
    
}

-(void)request:(PHPublisherContentRequest *)request contentDidFailWithError:(NSError *)error
{
    NSDictionary *errorData = [NSDictionary dictionaryWithObjectsAndKeys:
                               [NSNumber numberWithInt:error.code],@"code",
                               error.localizedDescription,@"description",
                               nil];
    NSDictionary *messageDictionary = [NSDictionary dictionaryWithObjectsAndKeys:
                                       [NSNumber numberWithInt:request.hashCode],@"hash",
                                       @"error", @"name",
                                       errorData,@"data", 
                                       nil];
    NSString *messageJSON = [self.writer stringWithObject:messageDictionary];
    UnitySendMessage("PlayHavenManager", "HandleNativeEvent", [messageJSON cStringUsingEncoding:NSUTF8StringEncoding]);
}

-(void)requestDidGetContent:(PHPublisherContentRequest *)request
{
    NSDictionary *pDictionary = [NSDictionary dictionaryWithObjectsAndKeys:
                                 request.placement,@"placement",
                                 nil];
    
    NSDictionary *messageDictionary = [NSDictionary dictionaryWithObjectsAndKeys:
                                       [NSNumber numberWithInt:request.hashCode],@"hash",
                                       @"gotcontent", @"name",
                                       pDictionary,@"data",
                                       nil];
    NSString *messageJSON = [self.writer stringWithObject:messageDictionary];
    UnitySendMessage("PlayHavenManager", "HandleNativeEvent", [messageJSON cStringUsingEncoding:NSUTF8StringEncoding]);
}

-(void)request:(PHPublisherContentRequest *)request contentWillDisplay:(PHContent *)content
{
    NSDictionary *messageDictionary = [NSDictionary dictionaryWithObjectsAndKeys:
                                       [NSNumber numberWithInt:request.hashCode],@"hash",
                                       @"willdisplay", @"name",
                                       [NSDictionary dictionary],@"data", 
                                       nil];
    NSString *messageJSON = [self.writer stringWithObject:messageDictionary];
    UnitySendMessage("PlayHavenManager", "HandleNativeEvent", [messageJSON cStringUsingEncoding:NSUTF8StringEncoding]);    
}

-(void)request:(PHPublisherContentRequest *)request contentDidDisplay:(PHContent *)content
{
    NSDictionary *messageDictionary = [NSDictionary dictionaryWithObjectsAndKeys:
                                       [NSNumber numberWithInt:request.hashCode],@"hash",
                                       @"diddisplay", @"name",
                                       [NSDictionary dictionary],@"data", 
                                       nil];
    NSString *messageJSON = [self.writer stringWithObject:messageDictionary];
    UnitySendMessage("PlayHavenManager", "HandleNativeEvent", [messageJSON cStringUsingEncoding:NSUTF8StringEncoding]);        
}

//-(void)requestContentDidDismiss:(PHPublisherContentRequest *)request
-(void)request:(PHPublisherContentRequest *)request contentDidDismissWithType:(PHPublisherContentDismissType*)type
{
	NSDictionary *dismissRepresentation = [NSDictionary dictionaryWithObjectsAndKeys:
											type, @"type",
											nil];
    NSDictionary *messageDictionary = [NSDictionary dictionaryWithObjectsAndKeys:
                                       [NSNumber numberWithInt:request.hashCode],@"hash",
                                       @"dismiss", @"name",
                                       dismissRepresentation,@"data", 
                                       nil];
    NSString *messageJSON = [self.writer stringWithObject:messageDictionary];
    UnitySendMessage("PlayHavenManager", "HandleNativeEvent", [messageJSON cStringUsingEncoding:NSUTF8StringEncoding]);  
}

-(void)request:(PHPublisherContentRequest *)request unlockedReward:(PHReward *)reward
{
	NSArray *keys = [NSArray arrayWithObjects:@"name",@"quantity",@"receipt",nil];
	NSDictionary *rewardRepresentation = [reward dictionaryWithValuesForKeys:keys];
	NSDictionary *messageDictionary = [NSDictionary dictionaryWithObjectsAndKeys:
								   [NSNumber numberWithInt:request.hashCode],@"hash",
								   @"reward", @"name",
								   rewardRepresentation,@"data", 
								   nil];
  NSString *messageJSON = [self.writer stringWithObject:messageDictionary];
  UnitySendMessage("PlayHavenManager", "HandleNativeEvent", [messageJSON cStringUsingEncoding:NSUTF8StringEncoding]); 
}

-(void)request:(PHPublisherContentRequest *)request makePurchase:(PHPurchase *)purchase
{
	self->currentPurchase = [purchase retain];
	NSArray *keys = [NSArray arrayWithObjects:@"productIdentifier",@"quantity",@"receipt",nil];
	NSDictionary *purchaseRepresentation = [purchase dictionaryWithValuesForKeys:keys];
	NSDictionary *messageDictionary = [NSDictionary dictionaryWithObjectsAndKeys:
								   [NSNumber numberWithInt:request.hashCode],@"hash",
								   @"purchasePresentation", @"name",
								   purchaseRepresentation,@"data", 
								   nil];
	NSString *messageJSON = [self.writer stringWithObject:messageDictionary];
	UnitySendMessage("PlayHavenManager", "HandleNativeEvent", [messageJSON cStringUsingEncoding:NSUTF8StringEncoding]); 
}
      
-(void)productPurchaseResolution:(int)action
{
	if (currentPurchase != nil)
		[currentPurchase reportResolution:(PHPurchaseResolutionType)action];
	[currentPurchase release];
	currentPurchase = nil;
}
          
+(void)cancelRequestWithHashCode:(int)hashCode
{
	int result = [PHAPIRequest cancelRequestWithHashCode:hashCode];
	if (result == 1) // OK
	{
		UnitySendMessage("PlayHavenManager", "RequestCancelSuccess", [[NSString stringWithFormat:@"%d", hashCode] cStringUsingEncoding:NSUTF8StringEncoding]);
	}
	else // 0 - hash not found
	{
		UnitySendMessage("PlayHavenManager", "RequestCancelFailed", [[NSString stringWithFormat:@"%d", hashCode] cStringUsingEncoding:NSUTF8StringEncoding]);
	}
}

@end

NSString* CreatePHUnityNSString(const char* string){
    if (string) {
        return [NSString stringWithUTF8String:string];
    } else {
        return @"";
    }
}

extern "C" {
    void _PlayHavenOpenRequest(const int hash, const char* token, const char* secret, const char* customUDID){
        PHPublisherOpenRequest *request = [PHPublisherOpenRequest 
                                           requestForApp:CreatePHUnityNSString(token)
                                           secret:CreatePHUnityNSString(secret)];
        request.delegate = [PHUnityIntegration sharedIntegration];
        request.hashCode = hash;
		#ifdef OPENUDID_SUPPORT
		if (customUDID != nil)
			request.customUDID = CreatePHUnityNSString(customUDID);
		#endif
        [request send];
    }
    
    void _PlayHavenMetadataRequest(const int hash, const char* token, const char* secret, const char* placement){
        PHPublisherMetadataRequest *request = [PHPublisherMetadataRequest 
                                               requestForApp:CreatePHUnityNSString(token)
                                               secret:CreatePHUnityNSString(secret)
                                               placement:CreatePHUnityNSString(placement)
                                               delegate:[PHUnityIntegration sharedIntegration]];
        request.hashCode = hash;
        [request send];
    }
    
    void _PlayHavenContentRequest(const int hash, const char* token, const char* secret, const char* placement, const bool showsOverlayImmediately)
    {
        PHPublisherContentRequest *request = [PHPublisherContentRequest
                                               requestForApp:CreatePHUnityNSString(token)
                                               secret:CreatePHUnityNSString(secret)
                                               placement:CreatePHUnityNSString(placement)
                                               delegate:[PHUnityIntegration sharedIntegration]];
        request.showsOverlayImmediately = showsOverlayImmediately;
        request.hashCode = hash;
        [request send];
    }
    
    void _PlayHavenPreloadRequest(const int hash, const char* token, const char* secret, const char* placement)
    {
        PHPublisherContentRequest *request = [PHPublisherContentRequest
                                              requestForApp:CreatePHUnityNSString(token)
                                              secret:CreatePHUnityNSString(secret)
                                              placement:CreatePHUnityNSString(placement)
                                              delegate:[PHUnityIntegration sharedIntegration]];
        request.hashCode = hash;        
        [request preload];
    }

	void _PlayHavenCancelRequest(const int hash)
	{
		[PHUnityIntegration cancelRequestWithHashCode:hash];
	}
	
	void _PlayHavenProductPurchaseResolution(const int action)
	{
		[[PHUnityIntegration sharedIntegration] productPurchaseResolution:action];
	}
	
	void _PlayHavenIAPTrackingRequest(const char* token, const char* secret, const char* productId, const int quantity, const int resolution)
	{
		PHPublisherIAPTrackingRequest *request;
		request = [PHPublisherIAPTrackingRequest requestForApp:CreatePHUnityNSString(token)
												 secret:CreatePHUnityNSString(secret)
												 product:CreatePHUnityNSString(productId) 
												 quantity:quantity
												 resolution:(PHPurchaseResolutionType)resolution];
        [request send];
	}
	
	bool _PlayHavenOptOutStatus()
	{
		return [PHAPIRequest optOutStatus];
	}
	
	void _PlayHavenSetOptOutStatus(bool yesOrNo)
	{
		[PHAPIRequest setOptOutStatus:yesOrNo];
	}
}
