using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class MoPubAdManager : MonoBehaviour
{
    static bool bInitialized = false;
    public string RewardedVideoAdUnit;
    public string InterstitialAdUnit;

    public Text DebugText;


    void Awake()
    {
        if (!bInitialized)
        {
            bInitialized = true;
            MoPub.initializeRewardedVideo();
            MoPub.requestRewardedVideo(RewardedVideoAdUnit);

            MoPub.requestInterstitialAd(InterstitialAdUnit);


            MoPub.createBanner("b3067e46003b4a24b19397ec6038cba6", MoPubAdPosition.BottomCenter);
        }
    }

    void OnEnable()
    {
        // Listen to all events for illustration purposes
        MoPubManager.onAdFailedEvent += onAdFailedEvent;

        MoPubManager.onInterstitialLoadedEvent += onInterstitialLoadedEvent;
        MoPubManager.onInterstitialFailedEvent += onInterstitialFailedEvent;
        MoPubManager.onInterstitialShownEvent += onInterstitialShownEvent;
        MoPubManager.onInterstitialDismissedEvent += onInterstitialDismissedEvent;
        MoPubManager.onInterstitialExpiredEvent += onInterstitialExpiredEvent;

        MoPubManager.onRewardedVideoLoadedEvent += onRewardedVideoLoadedEvent;
        MoPubManager.onRewardedVideoFailedEvent += onRewardedVideoFailedEvent;
        MoPubManager.onRewardedVideoExpiredEvent += onRewardedVideoExpiredEvent;
        MoPubManager.onRewardedVideoShownEvent += onRewardedVideoShownEvent;
        MoPubManager.onRewardedVideoFailedToPlayEvent += onRewardedVideoFailedToPlayEvent;
        MoPubManager.onRewardedVideoClosedEvent += onRewardedVideoClosedEvent;
    }

    void OnDisable()
    {
        // Listen to all events for illustration purposes
        MoPubManager.onAdFailedEvent -= onAdFailedEvent;

        MoPubManager.onInterstitialLoadedEvent -= onInterstitialLoadedEvent;
        MoPubManager.onInterstitialFailedEvent -= onInterstitialFailedEvent;
        MoPubManager.onInterstitialShownEvent -= onInterstitialShownEvent;
        MoPubManager.onInterstitialDismissedEvent -= onInterstitialDismissedEvent;
        MoPubManager.onInterstitialExpiredEvent -= onInterstitialExpiredEvent;

        MoPubManager.onRewardedVideoLoadedEvent -= onRewardedVideoLoadedEvent;
        MoPubManager.onRewardedVideoFailedEvent -= onRewardedVideoFailedEvent;
        MoPubManager.onRewardedVideoExpiredEvent -= onRewardedVideoExpiredEvent;
        MoPubManager.onRewardedVideoShownEvent -= onRewardedVideoShownEvent;
        MoPubManager.onRewardedVideoFailedToPlayEvent -= onRewardedVideoFailedToPlayEvent;
        MoPubManager.onRewardedVideoClosedEvent -= onRewardedVideoClosedEvent;
    }

    void onAdFailedEvent()
    {
        DebugText.text += "\n" + "onAdFailedEvent";
    }

    void onInterstitialLoadedEvent()
    {
        DebugText.text += "\n" + "onInterstitialLoadedEvent";
    }

    void onInterstitialShownEvent()
    {
        DebugText.text += "\n" + "onInterstitialShownEvent";
    }

    void onInterstitialFailedEvent()
    {
        DebugText.text += "\n" + "onInterstitialFailedEvent";
        MoPub.requestInterstitialAd(InterstitialAdUnit);
    }

    void onInterstitialDismissedEvent()
    {
        DebugText.text += "\n" + "onInterstitialDismissedEvent";
        MoPub.requestInterstitialAd(InterstitialAdUnit);
    }

    void onInterstitialExpiredEvent()
    {
        DebugText.text += "\n" + "onInterstitialExpiredEvent";
    }

    void onRewardedVideoLoadedEvent(string adUnitId)
    {
        DebugText.text += "\n" + "onRewardedVideoLoadedEvent";
    }

    void onRewardedVideoFailedEvent(string adUnitId)
    {
        DebugText.text += "\n" + "onRewardedVideoFailedEvent";
    }

    void onRewardedVideoExpiredEvent(string adUnitId)
    {
        DebugText.text += "\n" + "onRewardedVideoExpiredEvent";
    }

    void onRewardedVideoShownEvent(string adUnitId)
    {
        DebugText.text += "\n" + "onRewardedVideoShownEvent";
    }

    void onRewardedVideoFailedToPlayEvent(string adUnitId)
    {
        DebugText.text += "\n" + "onRewardedVideoFailedToPlayEvent";
    }

    void onRewardedVideoClosedEvent(string adUnitId)
    {
        DebugText.text += "\n" + "onRewardedVideoFailedToPlayEvent";
    }


    public void ShowRewardedVideo()
    {
			MoPub.showRewardedVideo(RewardedVideoAdUnit);
    }

    public void ShowInterstitial()
    {
            MoPub.showInterstitialAd(InterstitialAdUnit);
    }
}
