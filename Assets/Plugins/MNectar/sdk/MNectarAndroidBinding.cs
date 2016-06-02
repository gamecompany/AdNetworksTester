#if UNITY_ANDROID
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;



public enum MoPubAdPosition
{
	TopLeft,
	TopCenter,
	TopRight,
	Centered,
	BottomLeft,
	BottomCenter,
	BottomRight
}

public enum MoPubLocationAwareness
{
	TRUNCATED,
	DISABLED,
	NORMAL
}



public class MNectarAndroidBinding
{
	private static AndroidJavaObject _plugin;
	
	
	static MNectarAndroidBinding()
	{
		if( Application.platform != RuntimePlatform.Android )
			return;
		
		using( var pluginClass = new AndroidJavaClass( "com.mnectar.android.unity.MnectarUnityPlugin" ) )
			_plugin = pluginClass.CallStatic<AndroidJavaObject>( "getInstance" );
	}

	
	// Starts loading rewardable ad
	public static void requestRewardable( string adUnitId)
	{
		if( Application.platform != RuntimePlatform.Android )
			return;
		
		_plugin.Call( "requestRewardableAd", adUnitId );
	}
	
	
	// If an interstitial ad is loaded this will take over the screen and show the ad
	public static void showRewardable( string adUnitId )
	{
		if( Application.platform != RuntimePlatform.Android )
			return;
		
		_plugin.Call( "showRewardableAd", adUnitId );
	}
	
	
	public static bool isRewardableReady( string adUnitId )
	{
		if( Application.platform != RuntimePlatform.Android )
			return false;
		
		return _plugin.Call<bool>( "isRewardableAdReady", adUnitId );
	}
	public static void setPrefetch(string adUnitId, bool prefetch)
	{
		if( Application.platform != RuntimePlatform.Android )
			return;
		
		_plugin.Call( "setPrefetch", adUnitId, prefetch );
	}
	public static void initAdUnit(string adUnitId, Dictionary<string, string> parameters)
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		using(AndroidJavaObject obj_HashMap = new AndroidJavaObject("java.util.HashMap"))
		{
			// Call 'put' via the JNI instead of using helper classes to avoid:
			//  "JNI: Init'd AndroidJavaObject with null ptr!"
			IntPtr method_Put = AndroidJNIHelper.GetMethodID(obj_HashMap.GetRawClass(), "put",
			                                                 "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
			
			object[] args = new object[2];
			foreach(KeyValuePair<string, string> kvp in parameters)
			{
				using(AndroidJavaObject k = new AndroidJavaObject("java.lang.String", kvp.Key))
				{
					using(AndroidJavaObject v = new AndroidJavaObject("java.lang.String", kvp.Value))
					{
						args[0] = k;
						args[1] = v;
						AndroidJNI.CallObjectMethod(obj_HashMap.GetRawObject(),
						                            method_Put, AndroidJNIHelper.CreateJNIArgArray(args));
					}
				}
			}
			_plugin.Call( "initAdUnit", adUnitId, obj_HashMap);
		}
	}

	// Starts loading rewardable ad
	public static void requestInterstitial( string adUnitId)
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "requestInterstitialAd", adUnitId );
	}


	// If an interstitial ad is loaded this will take over the screen and show the ad
	public static void showInterstitial( string adUnitId )
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "showInterstitialAd", adUnitId );
	}


	public static bool isInterstitialReady( string adUnitId )
	{
		if( Application.platform != RuntimePlatform.Android )
			return false;

		return _plugin.Call<bool>( "isInterstitialAdReady", adUnitId );
	}
	public static void setInterstitialPrefetch(string adUnitId, bool prefetch)
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "setInterstitialPrefetch", adUnitId, prefetch );
	}
	public static void initInterstitialAdUnit(string adUnitId, Dictionary<string, string> parameters)
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		using(AndroidJavaObject obj_HashMap = new AndroidJavaObject("java.util.HashMap"))
		{
			// Call 'put' via the JNI instead of using helper classes to avoid:
			//  "JNI: Init'd AndroidJavaObject with null ptr!"
			IntPtr method_Put = AndroidJNIHelper.GetMethodID(obj_HashMap.GetRawClass(), "put",
				"(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");

			object[] args = new object[2];
			foreach(KeyValuePair<string, string> kvp in parameters)
			{
				using(AndroidJavaObject k = new AndroidJavaObject("java.lang.String", kvp.Key))
				{
					using(AndroidJavaObject v = new AndroidJavaObject("java.lang.String", kvp.Value))
					{
						args[0] = k;
						args[1] = v;
						AndroidJNI.CallObjectMethod(obj_HashMap.GetRawObject(),
							method_Put, AndroidJNIHelper.CreateJNIArgArray(args));
					}
				}
			}
			_plugin.Call( "initInterstitialAdUnit", adUnitId, obj_HashMap);
		}
	}

}
#endif
