//
//  MNectarManager.m
//
//
//  Created by Ian Reiss on 8/28/2015.
//  Copyright (c) 2015 mNectar Inc. All rights reserved.
//

#import "MNectarManager.h"
#import "UnityAppController+UnityInterface.h"

void UnitySendMessage( const char * className, const char * methodName, const char * param );

UIViewController *UnityGetGLViewController();

@interface MNectarManager()

@property NSMutableDictionary* params;

@end

@interface MNRewardable(Plugin)

@property(nonatomic, strong) NSString *adUnitId;

@end

@interface MNInterstitial(Plugin)

@property(nonatomic, strong) NSString *adUnitId;

@end

static MNectarManager *sharedManager = nil;


@implementation MNectarManager


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

+ (MNectarManager*)sharedManager
{
    
    if( !sharedManager )
        sharedManager = [[MNectarManager alloc] init];
    
    return sharedManager;
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Public

- (void)requestRewardable:(NSString*)adUnitId
{
    // this will return nil if there is already a load in progress
    MNRewardable *rewardable = [MNRewardable rewardableForAdUnitId:adUnitId];
    
    rewardable.delegate = self;
    [rewardable loadAd];
}

- (void)showRewardable:(NSString*)adUnitId
{
    MNRewardable *rewardable = [MNRewardable rewardableForAdUnitId:adUnitId];
    if( ![rewardable isAdReady])
    {
        NSLog( @"rewardable ad is not yet loaded" );
        return;
    }
    
    [rewardable showAdFromViewController:UnityGetGLViewController()];
}


- (BOOL)isRewardableReady:(NSString*)adUnitId
{
    MNRewardable *rewardable = [MNRewardable rewardableForAdUnitId:adUnitId];
    
    return [rewardable isAdReady];
}

- (void) setPrefetch: (bool) prefetch forAdUnit: (NSString*)adUnitId
{
    MNRewardable *rewardable = [MNRewardable rewardableForAdUnitId:adUnitId];
    [rewardable setPrefetch: prefetch];
}

- (void)initAdUnit:(NSString*)adUnitId
{
    MNRewardable *rewardable = [MNRewardable rewardableForAdUnitId:adUnitId parameters: self.params];
    self.params = nil;
}

- (void)requestInterstitial:(NSString*)adUnitId
{
    // this will return nil if there is already a load in progress
    MNInterstitial *interstitial = [MNInterstitial interstitialForAdUnitId:adUnitId];
    
    interstitial.delegate = self;
    [interstitial loadAd];
}

- (void)showInterstitial:(NSString*)adUnitId
{
    MNInterstitial *interstitial = [MNInterstitial interstitialForAdUnitId:adUnitId];
    if( ![interstitial isAdReady])
    {
        NSLog( @"interstitial ad is not yet loaded" );
        return;
    }
    
    
    [interstitial showAdFromViewController:UnityGetGLViewController()];
}


- (BOOL)isInterstitialReady:(NSString*)adUnitId
{
    MNInterstitial *interstitial = [MNInterstitial interstitialForAdUnitId:adUnitId];
    
    return [interstitial isAdReady];
}

- (void) setInterstitialPrefetch: (bool) prefetch forAdUnit: (NSString*)adUnitId
{
    MNInterstitial *interstitial = [MNInterstitial interstitialForAdUnitId:adUnitId];
    [interstitial setPrefetch: prefetch];
}

- (void)initInterstitialAdUnit:(NSString*)adUnitId
{
    MNInterstitial *interstitial = [MNInterstitial interstitialForAdUnitId:adUnitId parameters: self.params];
    self.params = nil;
}

- (void) addCustomParameterKey:(NSString*)  key value:(NSString*) value
{
    if (!self.params) {
        self.params = [NSMutableDictionary new];
    }
    [self.params setValue: value forKey: key];
}

- (void) updateCustomParameterKey:(NSString*) key value:(NSString*) value adUnit:(NSString *) adUnitId
{
    MNRewardable *rewardable = [MNRewardable rewardableForAdUnitId:adUnitId];
    if (rewardable.parameters){
        [rewardable.parameters setValue: value forKey: key];
    }
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - MPInterstitialAdManagerDelegate

- (void)presentInterstitialFromViewController:(UIViewController *)controller
{
    
}

///////////////////////////////////////////////////////////////////////////////////////////////////

#pragma mark - MNRewardableDelegate

- (void)rewardableDidLoad:(MNRewardable *)rewardable
{
    UnitySendMessage( "MNectarManager", "onRewardableLoaded", rewardable.adUnitId.UTF8String );
}

- (void)rewardableDidFail:(MNRewardable*)rewardable
{
    UnitySendMessage( "MNectarManager", "onRewardableFailed", rewardable.adUnitId.UTF8String );
}

- (void)rewardableWillAppear:(MNRewardable *)rewardable
{
    UnitySendMessage( "MNectarManager", "onRewardableWillShow", rewardable.adUnitId.UTF8String );
}

- (void)rewardableWillDismiss:(MNRewardable *)rewardable
{
    UnityPause(0);
    UnitySendMessage( "MNectarManager", "onRewardableWillDismiss", rewardable.adUnitId.UTF8String );
}

- (void)rewardableDidAppear:(MNRewardable *)rewardable
{
    UnitySendMessage( "MNectarManager", "onRewardableDidShow", rewardable.adUnitId.UTF8String );
    UnityPause(1);
}

- (void)rewardableDidDismiss:(MNRewardable *)rewardable
{
    UnitySendMessage( "MNectarManager", "onRewardableDidDismiss", rewardable.adUnitId.UTF8String );
}

- (void)rewardableShouldRewardUser:(MNRewardable *)rewardable reward:(MNReward *)reward
{
    NSString *strdata =[NSString stringWithFormat:@"%@ {\"type\": \"%@\", \"amount\": \"%@\"}", rewardable.adUnitId, reward.type, reward.amount];
    
    UnitySendMessage( "MNectarManager", "onRewardableShouldRewardUser", strdata.UTF8String );
}

///////////////////////////////////////////////////////////////////////////////////////////////////

#pragma mark - MNInterstitialDelegate

- (void)interstitialDidLoad:(MNInterstitial *)interstitial
{
    UnitySendMessage( "MNectarManager", "onInterstitialLoaded", interstitial.adUnitId.UTF8String );
}

- (void)interstitialDidFail:(MNInterstitial*)interstitial
{
    UnitySendMessage( "MNectarManager", "onInterstitialFailed", interstitial.adUnitId.UTF8String );
}

- (void)interstitialWillAppear:(MNInterstitial *)interstitial
{
    UnitySendMessage( "MNectarManager", "onInterstitialWillShow", interstitial.adUnitId.UTF8String );
}

- (void)interstitialWillDismiss:(MNInterstitial *)interstitial
{
    UnityPause(0);
    UnitySendMessage( "MNectarManager", "onInterstitialWillDismiss", interstitial.adUnitId.UTF8String );
}

- (void)interstitialDidAppear:(MNInterstitial *)interstitial
{
    UnitySendMessage( "MNectarManager", "onInterstitialDidShow", interstitial.adUnitId.UTF8String );
    UnityPause(1);
}

- (void)interstitialDidDismiss:(MNInterstitial *)interstitial
{
    UnitySendMessage( "MNectarManager", "onInterstitialDidDismiss", interstitial.adUnitId.UTF8String );
}



@end
