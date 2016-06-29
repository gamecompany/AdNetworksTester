using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if USE_CHARTBOOST
using ChartboostSDK;
#endif

public static class ChartboostAdNetwork
{
#if USE_CHARTBOOST
	[RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        Chartboost.Create();
		Chartboost.cacheRewardedVideo(CBLocation.locationFromName("Ad1Launch"));
        UIDebugLog.I["Chartboost"].Update("initialize - " + Chartboost.isInitialized());
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

    public static bool PlayRewardedAd(UnityEngine.Events.UnityAction onAdPlay, UnityEngine.Events.UnityAction adNotAviable)
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

