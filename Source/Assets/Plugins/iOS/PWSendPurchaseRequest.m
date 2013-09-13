//
//  PWSendPurchaseRequest.m
//  PushNotificationManager
//
//  Created by Alexander Anisimov on 8/2/13.
//
//

#import "PWSendPurchaseRequest.h"

@implementation PWSendPurchaseRequest

- (NSString *) methodName {
	return @"setPurchase";
}

- (NSDictionary *) requestDictionary {
	NSMutableDictionary *dict = [self baseDictionary];
	
	[dict setObject:_productIdentifier forKey:@"productIdentifier"];
	[dict setObject:[NSNumber numberWithInt:_quantity] forKey:@"quantity"];
	
	NSDateFormatter *df = [[NSDateFormatter alloc] init];
	[df setLocale:[[[NSLocale alloc] initWithLocaleIdentifier:@"en_US_POSIX"] autorelease]];
	[df setDateFormat:@"yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"];
    [df setTimeZone:[NSTimeZone timeZoneForSecondsFromGMT:0]];
	
	[dict setObject:[df stringFromDate:_transactionDate] forKey:@"transactionDate"];
	
	[df release];
	
	return dict;
}

- (void)dealloc {
	[_productIdentifier release];
	[_transactionDate release];
	[super dealloc];
}

@end
