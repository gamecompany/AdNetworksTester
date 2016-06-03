using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if USE_CHARTBOOST
using ChartboostSDK;
#endif

public static class ChartbootAdNetwork
{
#if USE_CHARTBOOST
	[RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
    	Chartboost.Create();
		Chartboost.cacheRewardedVideo(CBLocation.Default);
    }

	public static bool PlayAd(string placementName, bool isIncentivized)
    {
    	if (isIncentivized)
    	{
			return PlayChartBoostIncentivizedVideoAd();
		}
		else
		{
			return PlayChartBoostVideoAd();
		}
    }

	private static bool PlayChartBoostIncentivizedVideoAd()
    {
        if (Chartboost.hasRewardedVideo(CBLocation.Default)) 
		{
			Chartboost.showRewardedVideo(CBLocation.Default);
			Chartboost.cacheRewardedVideo(CBLocation.Default);

			return true;
		}
		return false;
	}

	private static bool PlayChartBoostVideoAd()
	{
        // Note: We can used named locations.
        // Chartboost.showInterstitial(CBLocation.locationFromName("CustomLocation"));
        // https://answers.chartboost.com/hc/en-us/articles/204888915
        
        if (Chartboost.hasInterstitial(CBLocation.Default)) 
		{
			Chartboost.showInterstitial(CBLocation.Default);
			Chartboost.cacheInterstitial(CBLocation.Default);

			return true;
		}
		return false;
	}

    public static bool PlayAd()
    {
        if (Chartboost.hasRewardedVideo(CBLocation.Default))
        {
            Chartboost.showRewardedVideo(CBLocation.Default);
            Chartboost.cacheRewardedVideo(CBLocation.Default);

            return true;
        }
        return false;
    }

    public static bool PlayAd(UnityEngine.Events.UnityAction onAdPlay, UnityEngine.Events.UnityAction adNotAviable)
    {
        if (Chartboost.hasRewardedVideo(CBLocation.Default))
        {
            Chartboost.showRewardedVideo(CBLocation.Default);
            Chartboost.cacheRewardedVideo(CBLocation.Default);
            // Callback
            if (null != onAdPlay) onAdPlay.Invoke();
            return true;
        }
        // Callback
        if (null != adNotAviable) adNotAviable.Invoke();

        return false;
    }

#endif

#if UNITY_EDITOR

    // CUSTOM COMPILER DEFINES
    const string UseChartboost = "USE_CHARTBOOST";	// when define is set, we use ads from Chartboost

	// Chartboost menus
	// ----------------

	[MenuItem("Ad Networks/Chartboost/Enable", false, 12)]
    private static void EnableChartboost()
    {
		AdNetworkSettingsHelper.SetEnabled(UseChartboost, true);
    }

	[MenuItem("Ad Networks/Chartboost/Enable", true)]
	private static bool EnableChartboostValidate()
    {
		return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseChartboost) != AdNetworkSettingsHelper.DefinedForPlatforms.All;
    }

	[MenuItem("Ad Networks/Chartboost/Disable", false, 13)]
    private static void DisableChartboost()
    {
		AdNetworkSettingsHelper.SetEnabled(UseChartboost, false);
    }

	[MenuItem("Ad Networks/Chartboost/Disable", true)]
	private static bool DisableChartboostValidate()
    {
		return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseChartboost) != AdNetworkSettingsHelper.DefinedForPlatforms.None;
    }
#endif
}

