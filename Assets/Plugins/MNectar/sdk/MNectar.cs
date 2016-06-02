using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_IPHONE || UNITY_ANDROID

#if UNITY_IPHONE
using MN = MNectarIOSBinding;
#elif UNITY_ANDROID
using MN = MNectarAndroidBinding;
#endif

public class MNectar
{
	// Starts loading rewardable ad
	public static void requestRewardable( string adUnitId )
	{
		MN.requestRewardable( adUnitId );
	}
	
	// If rewardable ad is loaded this will take over the screen and show the ad
	public static void showRewardable( string adUnitId )
	{
		MN.showRewardable( adUnitId );
	}
	
	public static void setPrefetch( string adUnitId, bool prefetch )
	{
		MN.setPrefetch( adUnitId, prefetch );
	}
	
	public static bool isRewardableReady( string adUnitId )
	{
		return MN.isRewardableReady( adUnitId );
	}
	
	public static void initAdUnit( string adUnitId )
	{
		initAdUnit( adUnitId, new Dictionary<string, string>() );
	}
	
	public static void initAdUnit(string adUnitId, Dictionary<string, string> parameters)
	{
		MN.initAdUnit (adUnitId, parameters);
	}
	// Starts loading rewardable ad
	public static void requestInterstitial( string adUnitId )
	{
		MN.requestInterstitial( adUnitId );
	}
	
	// If rewardable ad is loaded this will take over the screen and show the ad
	public static void showInterstitial( string adUnitId )
	{
		MN.showInterstitial( adUnitId );
	}
	
	public static void setInterstitialPrefetch( string adUnitId, bool prefetch )
	{
		MN.setInterstitialPrefetch( adUnitId, prefetch );
	}
	
	public static bool isInterstitialReady( string adUnitId )
	{
		return MN.isInterstitialReady( adUnitId );
	}
	
	public static void initInterstitialAdUnit( string adUnitId )
	{
		initInterstitialAdUnit( adUnitId, new Dictionary<string, string>() );
	}
	
	public static void initInterstitialAdUnit(string adUnitId, Dictionary<string, string> parameters)
	{
		MN.initInterstitialAdUnit (adUnitId, parameters);
	}

}

#endif