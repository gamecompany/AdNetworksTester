using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

#if UNITY_IPHONE

public class MNectarIOSBinding
{
	[DllImport("__Internal")]
	private static extern void _mnRequestRewardable( string adUnitId );
	
	// Starts loading rewardable ad
	public static void requestRewardable( string adUnitId )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_mnRequestRewardable( adUnitId );
	}
		
	[DllImport("__Internal")]
	private static extern void _mnShowRewardable( string adUnitId );
	
	// If rewardable ad is loaded this will take over the screen and show the ad
	public static void showRewardable( string adUnitId )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_mnShowRewardable( adUnitId );
	}

	[DllImport("__Internal")]
	private static extern void _mnSetPrefetch( string adUnitId, bool prefetch );

	public static void setPrefetch( string adUnitId, bool prefetch )
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_mnSetPrefetch (adUnitId, prefetch);
		}
	}

	[DllImport("__Internal")]
	private static extern bool _mnIsRewardableReady( string adUnitId );

	public static bool isRewardableReady( string adUnitId )
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			return _mnIsRewardableReady (adUnitId);
		else
			return false;
	}

	[DllImport("__Internal")]
	private static extern void _mnInitAdUnit( string adUnitId );

	public static void initAdUnit( string adUnitId, Dictionary<string, string> parameters)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			foreach (KeyValuePair<string, string> entry in parameters) {
				addCustomParameter (entry.Key, entry.Value); 
			}
			_mnInitAdUnit (adUnitId);
		}
	}

	[DllImport("__Internal")]
	private static extern void _mnAddCustomParameter( string key, string value );

	public static void addCustomParameter( string key, string value )
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_mnAddCustomParameter( key, value );
	}
	[DllImport("__Internal")]
	private static extern void _mnUpdateCustomParameter( string key, string value , string adUnitId );

	public static void updateCustomParameter( string key, string value, string adUnitId )
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_mnUpdateCustomParameter(key, value, adUnitId);
	}

	[DllImport("__Internal")]
	private static extern void _mnRequestInterstitial( string adUnitId );
	
	// Starts loading rewardable ad
	public static void requestInterstitial( string adUnitId )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_mnRequestInterstitial( adUnitId );
	}
	
	[DllImport("__Internal")]
	private static extern void _mnShowInterstitial( string adUnitId );
	
	// If rewardable ad is loaded this will take over the screen and show the ad
	public static void showInterstitial( string adUnitId )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_mnShowInterstitial( adUnitId );
	}
	
	[DllImport("__Internal")]
	private static extern void _mnSetInterstitialPrefetch( string adUnitId, bool prefetch );
	
	public static void setInterstitialPrefetch( string adUnitId, bool prefetch )
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_mnSetInterstitialPrefetch (adUnitId, prefetch);
		}
	}
	
	[DllImport("__Internal")]
	private static extern bool _mnIsInterstitialReady( string adUnitId );
	
	public static bool isInterstitialReady( string adUnitId )
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			return _mnIsInterstitialReady (adUnitId);
		else
			return false;
	}
	
	[DllImport("__Internal")]
	private static extern void _mnInitInterstitialAdUnit( string adUnitId );
	
	public static void initInterstitialAdUnit( string adUnitId, Dictionary<string, string> parameters)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			foreach (KeyValuePair<string, string> entry in parameters) {
				addCustomParameter (entry.Key, entry.Value); 
			}
			_mnInitInterstitialAdUnit (adUnitId);
		}
	}

}
#endif