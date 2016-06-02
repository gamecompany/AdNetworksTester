using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using MNectarMiniJSON;

#if UNITY_IPHONE || UNITY_ANDROID
public class MNectarManager : MonoBehaviour
{
	// Fired when rewardable ad is loaded and ready to be shown
	public static event Action<string> onRewardableLoadedEvent;
	
	// Fired when rewardable ad fails to load
	public static event Action<string> onRewardableFailedEvent;
	
	// Fired when an interstitial ad will dismiss
	public static event Action<string> onRewardableWillDismissEvent;
	
	// Fired when rewardable ad will display
	public static event Action<string> onRewardableWillShowEvent;
	
	// Fired when an interstitial ad is dismissed
	public static event Action<string> onRewardableDidDismissEvent;
	
	// Fired when rewardable ad is displayed
	public static event Action<string> onRewardableDidShowEvent;
	
	// Fired when rewardable ad is clicked
	public static event Action<string> onRewardableClickedEvent;
	
	// Fired when rewardable ad should reward the user
	public static event Action<string, string, string> onRewardableShouldRewardUserEvent;

	public static event Action<string> onInterstitialLoadedEvent;
	
	// Fired when rewardable ad fails to load
	public static event Action<string> onInterstitialFailedEvent;
	
	// Fired when an interstitial ad will dismiss
	public static event Action<string> onInterstitialWillDismissEvent;
	
	// Fired when rewardable ad will display
	public static event Action<string> onInterstitialWillShowEvent;
	
	// Fired when an interstitial ad is dismissed
	public static event Action<string> onInterstitialDidDismissEvent;
	
	// Fired when rewardable ad is displayed
	public static event Action<string> onInterstitialDidShowEvent;
	
	// Fired when rewardable ad is clicked
	public static event Action<string> onInterstitialClickedEvent;


	static MNectarManager()
	{
		var type = typeof( MNectarManager );
		try
		{
			// first we see if we already exist in the scene
			var obj = FindObjectOfType( type ) as MonoBehaviour;
			if( obj != null )
				return;

			// create a new GO for our manager
			var managerGO = new GameObject( type.ToString() );
			managerGO.AddComponent( type );
			DontDestroyOnLoad( managerGO );
		}
		catch( UnityException )
		{
			Debug.LogWarning( "It looks like you have the " + type + " on a GameObject in your scene. Please remove the script from your scene." );
		}
	}

	void onRewardableLoaded( string adUnitId )
	{
		if( onRewardableLoadedEvent != null )
			onRewardableLoadedEvent(adUnitId);
	}
	
	void onRewardableFailed( string adUnitId )
	{
		if( onRewardableFailedEvent != null )
			onRewardableFailedEvent(adUnitId);
	}
	
	void onRewardableWillDismiss( string adUnitId )
	{
		if( onRewardableWillDismissEvent != null )
			onRewardableWillDismissEvent(adUnitId);
	}
	
	void onRewardableWillShow( string adUnitId )
	{
		if ( onRewardableWillShowEvent != null )
			onRewardableWillShowEvent(adUnitId);
	}
	
	void onRewardableDidDismiss( string adUnitId )
	{
		if( onRewardableDidDismissEvent != null )
			onRewardableDidDismissEvent(adUnitId);
	}
	
	void onRewardableDidShow( string adUnitId )
	{
		if ( onRewardableDidShowEvent != null )
			onRewardableDidShowEvent(adUnitId);
	}
	
	void onRewardableClicked( string adUnitId )
	{
		if ( onRewardableClickedEvent != null )
			onRewardableClickedEvent(adUnitId);
	}

	void onRewardableShouldRewardUser(string reward )
	{
		Debug.Log (reward);
		string adUnitID = reward.Substring(0,reward.IndexOf(" {"));
		string rewarddata = reward.Substring (adUnitID.Length + 1);
		string rewardType = "";
		string rewardAmount = "";
		Debug.Log (rewarddata);

		if (onRewardableShouldRewardUserEvent != null) {
			var obj = MNectarMiniJSON.Json.Deserialize( rewarddata ) as Dictionary<string, object>;
			if( obj == null ) {
				return;
			}

			if( obj.ContainsKey( "type" ) ) {
				rewardType = obj["type"].ToString();
			}

			if( obj.ContainsKey( "amount" ) ) {
				rewardAmount = obj["amount"].ToString();
			}

			onRewardableShouldRewardUserEvent (adUnitID, rewardType, rewardAmount );
		}
	}

	void onInterstitialLoaded( string adUnitId )
	{
		if( onInterstitialLoadedEvent != null )
			onInterstitialLoadedEvent(adUnitId);
	}
	
	void onInterstitialFailed( string adUnitId )
	{
		if( onInterstitialFailedEvent != null )
			onInterstitialFailedEvent(adUnitId);
	}
	
	void onInterstitialWillDismiss( string adUnitId )
	{
		if( onInterstitialWillDismissEvent != null )
			onInterstitialWillDismissEvent(adUnitId);
	}
	
	void onInterstitialWillShow( string adUnitId )
	{
		if ( onInterstitialWillShowEvent != null )
			onInterstitialWillShowEvent(adUnitId);
	}
	
	void onInterstitialDidDismiss( string adUnitId )
	{
		if( onInterstitialDidDismissEvent != null )
			onInterstitialDidDismissEvent(adUnitId);
	}
	
	void onInterstitialDidShow( string adUnitId )
	{
		if ( onInterstitialDidShowEvent != null )
			onInterstitialDidShowEvent(adUnitId);
	}
	
	void onInterstitialClicked( string adUnitId )
	{
		if ( onInterstitialClickedEvent != null )
			onInterstitialClickedEvent(adUnitId);
	}
}


#endif
