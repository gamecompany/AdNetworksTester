using UnityEngine;
using System.Collections;

public class SuperSonicManager : MonoBehaviour
{
    static bool initialized = false;

    public string appKey; //for testing: 4d7447bd
    public UIDebugLog MyLog;

    void Awake()
    {
        if (!initialized)
        {
            initialized = true;
            Supersonic.Agent.start();
            //Init Rewarded Video
            string uniqueUserId = SystemInfo.deviceUniqueIdentifier; //for testing:
            Supersonic.Agent.initRewardedVideo(appKey, uniqueUserId);
            Supersonic.Agent.initInterstitial(appKey, uniqueUserId);
        }
    }

    public void ShowRewardedAd()
    {
        if (Supersonic.Agent.isRewardedVideoAvailable())
        {
            //Supersonic.Agent.showRewardedVideo();
            Supersonic.Agent.showRewardedVideo("rewardedVideoZone");
        }
        else
        {
            MyLog.Display("Rewarded Ad not available");
        }
    }

    public void ShowInterstitial()
    {
        if (Supersonic.Agent.isInterstitialReady())
        {
            Supersonic.Agent.showInterstitial();
        }
        else
        {
            MyLog.Display("IS Ad not available");
        }
    }

    void OnEnable()
    {
        AddRewardedEvents();
        AddInterstitialEvents();
    }

    void AddInterstitialEvents()
    {
        SupersonicEvents.onInterstitialInitSuccessEvent += InterstitialInitSuccessEvent;
        SupersonicEvents.onInterstitialInitFailedEvent += InterstitialInitFailEvent;
        SupersonicEvents.onInterstitialReadyEvent += InterstitialReadyEvent;
        SupersonicEvents.onInterstitialLoadFailedEvent += InterstitialLoadFailedEvent;
        SupersonicEvents.onInterstitialShowSuccessEvent += InterstitialShowSuccessEvent;
        SupersonicEvents.onInterstitialShowFailedEvent += InterstitialShowFailEvent;
        SupersonicEvents.onInterstitialClickEvent += InterstitialAdClickedEvent;
        SupersonicEvents.onInterstitialOpenEvent += InterstitialAdOpenedEvent;
        SupersonicEvents.onInterstitialCloseEvent += InterstitialAdClosedEvent;
    }

    void AddRewardedEvents()
    {
        SupersonicEvents.onRewardedVideoInitSuccessEvent += RewardedVideoInitSuccessEvent;
        SupersonicEvents.onRewardedVideoInitFailEvent += RewardedVideoInitFailEvent;
        SupersonicEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        SupersonicEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        SupersonicEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        SupersonicEvents.onVideoAvailabilityChangedEvent += VideoAvailabilityChangedEvent;
        SupersonicEvents.onVideoStartEvent += VideoStartEvent;
        SupersonicEvents.onVideoEndEvent += VideoEndEvent;
    }

    void OnDisable()
    {
        RemoveRewardedEvents();
        RemoveInterstitialEvents();
    }

    void RemoveInterstitialEvents()
    {
        SupersonicEvents.onInterstitialInitSuccessEvent -= InterstitialInitSuccessEvent;
        SupersonicEvents.onInterstitialInitFailedEvent -= InterstitialInitFailEvent;
        SupersonicEvents.onInterstitialReadyEvent -= InterstitialReadyEvent;
        SupersonicEvents.onInterstitialLoadFailedEvent -= InterstitialLoadFailedEvent;
        SupersonicEvents.onInterstitialShowSuccessEvent -= InterstitialShowSuccessEvent;
        SupersonicEvents.onInterstitialShowFailedEvent -= InterstitialShowFailEvent;
        SupersonicEvents.onInterstitialClickEvent -= InterstitialAdClickedEvent;
        SupersonicEvents.onInterstitialOpenEvent -= InterstitialAdOpenedEvent;
        SupersonicEvents.onInterstitialCloseEvent -= InterstitialAdClosedEvent;
    }

    void RemoveRewardedEvents()
    {
        SupersonicEvents.onRewardedVideoInitSuccessEvent -= RewardedVideoInitSuccessEvent;
        SupersonicEvents.onRewardedVideoInitFailEvent -= RewardedVideoInitFailEvent;
        SupersonicEvents.onRewardedVideoAdOpenedEvent -= RewardedVideoAdOpenedEvent;
        SupersonicEvents.onRewardedVideoAdRewardedEvent -= RewardedVideoAdRewardedEvent;
        SupersonicEvents.onRewardedVideoAdClosedEvent -= RewardedVideoAdClosedEvent;
        SupersonicEvents.onVideoAvailabilityChangedEvent -= VideoAvailabilityChangedEvent;
        SupersonicEvents.onVideoStartEvent -= VideoStartEvent;
        SupersonicEvents.onVideoEndEvent -= VideoEndEvent;
    }

    void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            Supersonic.Agent.onPause();
        }
        else
        {
            Supersonic.Agent.onResume();
        }
    }

    //Invoked when initialization of RewardedVideo has finished successfully.
    void RewardedVideoInitSuccessEvent()
    {
        MyLog.Display("RewardedVideoInitSuccessEvent");
    }

    //Invoked when RewardedVideo initialization process has failed. 
    //SupersonicError contains the reason for the failure. 
    void RewardedVideoInitFailEvent(SupersonicError error)
    {
        MyLog.Display("RewardedVideoInitFailEvent, code: " + error.getCode() + ", description : " + error.getDescription());

        Debug.Log("Init rewarded video error ");

    }

    //Invoked when the RewardedVideo ad view has opened.
    //Your Activity will lose focus. Please avoid performing heavy 
    //tasks till the video ad will be closed.
    void RewardedVideoAdOpenedEvent()
    {
        MyLog.Display("RewardedVideoAdOpenedEvent");
    }

    //Invoked when the RewardedVideo ad view is about to be closed.
    //Your activity will now regain its focus.
    void RewardedVideoAdClosedEvent()
    {
        MyLog.Display("RewardedVideoAdClosedEvent");
    }

    //Invoked when there is a change in the ad availability status.
    //@param - available - value will change to true when rewarded videos are available. 
    //You can then show the video by calling showRewardedVideo().
    //Value will change to false when no videos are available.
    void VideoAvailabilityChangedEvent(bool available)
    {
        //Change the in-app 'Traffic Driver' state according to availability.
        MyLog.Display("VideoAvailabilityChangedEvent to " + available);
    }

    //Invoked when the video ad starts playing.
    void VideoStartEvent()
    {
        MyLog.Display("VideoStartEvent");
    }

    //Invoked when the video ad finishes playing.
    void VideoEndEvent()
    {
        MyLog.Display("VideoEndEvent");
    }

    //Invoked when the user completed the video and should be rewarded. 
    //If using server-to-server callbacks you may ignore this events and wait for 
    //the callback from the Supersonic server.
    //@param - placement - placement object which contains the reward data
    void RewardedVideoAdRewardedEvent(SupersonicPlacement placement)
    {
        MyLog.Display("RewardedVideoAdRewardedEvent");
    }

    /*
     * Invoked when Interstitial initialization process completes successfully.
     */
    void InterstitialInitSuccessEvent()
    {
        MyLog.Display("InterstitialInitSuccessEvent");
        Supersonic.Agent.loadInterstitial();
    }
    /* 
     * Invoked when the initialization process has failed.
     * @param description - string - contains information about the failure.
     */
    void InterstitialLoadFailedEvent(SupersonicError error)
    {
        MyLog.Display("InterstitialLoadFailedEvent, code: " + error.getCode() + ", description : " + error.getDescription());
    }
    /* 
     * Invoked right before the Interstitial screen is about to open.
     */
    void InterstitialShowSuccessEvent()
    {
        MyLog.Display("InterstitialShowSuccessEvent");
    }
    /* 
     * Invoked when the ad fails to show.
     * @param description - string - contains information about the failure.
     */
    void InterstitialShowFailEvent(SupersonicError error)
    {
        MyLog.Display("InterstitialShowFailEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
    }
    /* 
     * Invoked upon ad availability change.
     * @param available - bool - true when interstitial ad is ready to be presented or false
     * otherwise.
     */
    void ISAvailability(bool available)
    {
        //Show the Ad if available value is true
        MyLog.Display("Interstitial Availability " + available);
    }
    /* 
     * Invoked when end user clicked on the interstitial ad
     */
    void InterstitialAdClickedEvent()
    {
        MyLog.Display("InterstitialAdClickedEvent");
    }
    /* 
     * Invoked when the interstitial ad closed and the user goes back to the application screen.
     */
    void InterstitialAdClosedEvent()
    {
        MyLog.Display("InterstitialAdClosedEvent");
        Supersonic.Agent.loadInterstitial();

    }

    void InterstitialAdOpenedEvent()
    {
        MyLog.Display("InterstitialAdOpenedEvent");
    }

    void InterstitialReadyEvent()
    {
        MyLog.Display("InterstitialReadyEvent");
    }

    void InterstitialInitFailEvent(SupersonicError error)
    {
        MyLog.Display("InterstitialInitFailEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
    }
}
