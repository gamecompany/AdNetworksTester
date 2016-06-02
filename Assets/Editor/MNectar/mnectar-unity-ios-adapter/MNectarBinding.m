#import "MNectarManager.h"

// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

// Starts loading rewardable ad
void _mnRequestRewardable( const char * adUnitId )
{
    [[MNectarManager sharedManager] requestRewardable:GetStringParam( adUnitId )];
}

// If rewardable ad is loaded this will take over the screen and show the ad
void _mnShowRewardable( const char * adUnitId )
{
	[[MNectarManager sharedManager] showRewardable:GetStringParam( adUnitId )];
}

bool _mnIsRewardableReady( const char * adUnitId )
{
    return [[MNectarManager sharedManager] isRewardableReady:GetStringParam( adUnitId )];
}

void _mnSetPrefetch(const char * adUnitId, bool prefetch)
{
    [[MNectarManager sharedManager] setPrefetch: prefetch forAdUnit:GetStringParam( adUnitId )];
}

void _mnInitAdUnit(const char * adUnitId)
{
    [[MNectarManager sharedManager] initAdUnit:GetStringParam( adUnitId )];
}



void _mnAddCustomParameter( const char * key, const char * value )
{
    [[MNectarManager sharedManager] addCustomParameterKey: GetStringParam(key) value: GetStringParam(value)];
}

void _mnUpdateCustomParameter( const char * key, const char * value , const char * adUnitId )
{
    [[MNectarManager sharedManager] updateCustomParameterKey: GetStringParam(key) value: GetStringParam(value) adUnit: GetStringParam(adUnitId)];
}

// Starts loading Interstitial ad
void _mnRequestInterstitial( const char * adUnitId )
{
    [[MNectarManager sharedManager] requestInterstitial:GetStringParam( adUnitId )];
}

// If Interstitial ad is loaded this will take over the screen and show the ad
void _mnShowInterstitial( const char * adUnitId )
{
    [[MNectarManager sharedManager] showInterstitial:GetStringParam( adUnitId )];
}

bool _mnIsInterstitialReady( const char * adUnitId )
{
    return [[MNectarManager sharedManager] isInterstitialReady:GetStringParam( adUnitId )];
}

void _mnSetInterstitialPrefetch(const char * adUnitId, bool prefetch)
{
    [[MNectarManager sharedManager] setInterstitialPrefetch: prefetch forAdUnit:GetStringParam( adUnitId )];
}

void _mnInitInterstitialAdUnit(const char * adUnitId)
{
    [[MNectarManager sharedManager] initInterstitialAdUnit:GetStringParam( adUnitId )];
}
