#import <Foundation/Foundation.h>
#import "MNRewardable.h"
#import "MNInterstitial.h"
#import "MNAdClient.h"

@interface MNectarManager : NSObject <MNRewardableDelegate, MNInterstitialDelegate>

+ (MNectarManager*)sharedManager;

- (void)requestRewardable:(NSString*)adUnitId;

- (void)showRewardable:(NSString*)adUnitId;

- (BOOL)isRewardableReady:(NSString*)adUnitId;

- (void) setPrefetch: (bool) prefetch forAdUnit: (NSString*)adUnitId;

- (void)initAdUnit:(NSString*)adUnitId;

- (void)requestInterstitial:(NSString*)adUnitId;

- (void)showInterstitial:(NSString*)adUnitId;

- (BOOL)isInterstitialReady:(NSString*)adUnitId;

- (void) setInterstitialPrefetch: (bool) prefetch forAdUnit: (NSString*)adUnitId;

- (void)initInterstitialAdUnit:(NSString*)adUnitId;

- (void) addCustomParameterKey:(NSString*) key value:(NSString*) value;

- (void) updateCustomParameterKey:(NSString*) key value:(NSString*) value adUnit:(NSString *) adUnitId;

@end
