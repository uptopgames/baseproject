//
//  PWSendPurchaseRequest.h
//  PushNotificationManager
//
//  Created by Alexander Anisimov on 8/2/13.
//
//

#import "PWRequest.h"

@interface PWSendPurchaseRequest : PWRequest

@property (nonatomic, copy) NSString *productIdentifier;
@property (nonatomic) NSInteger quantity;
@property (nonatomic, retain) NSDate *transactionDate;

@end
