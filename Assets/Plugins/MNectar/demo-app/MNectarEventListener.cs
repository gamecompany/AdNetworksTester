using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;


public class MNectarEventListener : MonoBehaviour
{
	[SerializeField] private Text ReadinessInfo; 
#if UNITY_ANDROID || UNITY_IPHONE

	void OnEnable()
	{
		MNectarManager.onRewardableLoadedEvent += onRewardableLoadedEvent;
		MNectarManager.onRewardableFailedEvent += onRewardableFailedEvent;
		MNectarManager.onRewardableWillShowEvent += onRewardableWillShowEvent;
		MNectarManager.onRewardableWillDismissEvent += onRewardableWillDismissEvent;
		MNectarManager.onRewardableDidShowEvent += onRewardableShownEvent;
		MNectarManager.onRewardableDidDismissEvent += onRewardableDismissedEvent;
		MNectarManager.onRewardableClickedEvent += onRewardableClickedEvent;
		MNectarManager.onRewardableShouldRewardUserEvent += onRewardableShouldRewardUserEvent;

		MNectarManager.onInterstitialLoadedEvent += onInterstitialLoadedEvent;
		MNectarManager.onInterstitialFailedEvent += onInterstitialFailedEvent;
		MNectarManager.onInterstitialWillShowEvent += onInterstitialWillShowEvent;
		MNectarManager.onInterstitialWillDismissEvent += onInterstitialWillDismissEvent;
		MNectarManager.onInterstitialDidShowEvent += onInterstitialShownEvent;
		MNectarManager.onInterstitialDidDismissEvent += onInterstitialDismissedEvent;
		MNectarManager.onInterstitialClickedEvent += onInterstitialClickedEvent;

	}
	
	void OnDisable()
	{
		MNectarManager.onRewardableLoadedEvent -= onRewardableLoadedEvent;
		MNectarManager.onRewardableFailedEvent -= onRewardableFailedEvent;
		MNectarManager.onRewardableWillShowEvent -= onRewardableWillShowEvent;
		MNectarManager.onRewardableWillDismissEvent -= onRewardableWillDismissEvent;
		MNectarManager.onRewardableDidShowEvent -= onRewardableShownEvent;
		MNectarManager.onRewardableDidDismissEvent -= onRewardableDismissedEvent;
		MNectarManager.onRewardableClickedEvent -= onRewardableClickedEvent;
		MNectarManager.onRewardableShouldRewardUserEvent -= onRewardableShouldRewardUserEvent;

		MNectarManager.onInterstitialLoadedEvent -= onInterstitialLoadedEvent;
		MNectarManager.onInterstitialFailedEvent -= onInterstitialFailedEvent;
		MNectarManager.onInterstitialWillShowEvent -= onInterstitialWillShowEvent;
		MNectarManager.onInterstitialWillDismissEvent -= onInterstitialWillDismissEvent;
		MNectarManager.onInterstitialDidShowEvent -= onInterstitialShownEvent;
		MNectarManager.onInterstitialDidDismissEvent -= onInterstitialDismissedEvent;
		MNectarManager.onInterstitialClickedEvent -= onInterstitialClickedEvent;

	}

	void onRewardableLoadedEvent(string adunit)
	{
		ReadinessInfo.text = "Ad ready";
		Debug.Log( "onRewardableLoadedEvent " + adunit );
		if (MNectar.isRewardableReady (adunit)) {
			Debug.Log ("rewardable is ready");
		}
	}

	void onRewardableFailedEvent(string adunit)
	{
		Debug.Log( "onRewardableFailedEvent" );
	}
	
	void onRewardableWillShowEvent(string adunit)
	{
		Debug.Log( "onRewardableWillShowEvent " + adunit );
	}
	
	void onRewardableWillDismissEvent(string adunit)
	{
		Debug.Log( "onRewardableWillDismissEvent" );
	}

	void onRewardableShownEvent(string adunit)
	{
		Debug.Log( "onRewardableShownEvent " + adunit );
	}
	
	void onRewardableDismissedEvent(string adunit)
	{
		Debug.Log( "onRewardableDismissedEvent" );
	}

	void onRewardableClickedEvent(string adunit)
	{
		Debug.Log( "onRewardableClickedEvent" );
	}

	void onRewardableShouldRewardUserEvent(string adunit, string rewardType, string rewardAmount )
	{
		Debug.Log ("onRewardableShouldRewardUser " + rewardAmount + " " + rewardType + " for ad unit: "+adunit);
	}

	void onInterstitialLoadedEvent(string adunit)
	{
		ReadinessInfo.text = "Ad ready";
		Debug.Log( "onInterstitialLoadedEvent " + adunit );
		if (MNectar.isInterstitialReady (adunit)) {
			Debug.Log ("rewardable is ready");
		}
	}

	void onInterstitialFailedEvent(string adunit)
	{
		Debug.Log( "onInterstitialFailedEvent" );
	}

	void onInterstitialWillShowEvent(string adunit)
	{
		Debug.Log( "onInterstitialWillShowEvent " + adunit );
	}

	void onInterstitialWillDismissEvent(string adunit)
	{
		Debug.Log( "onInterstitialWillDismissEvent" );
	}

	void onInterstitialShownEvent(string adunit)
	{
		Debug.Log( "onInterstitialShownEvent " + adunit );
	}

	void onInterstitialDismissedEvent(string adunit)
	{
		Debug.Log( "onInterstitialDismissedEvent" );
	}

	void onInterstitialClickedEvent(string adunit)
	{
		Debug.Log( "onInterstitialClickedEvent" );
	}

#endif
}


