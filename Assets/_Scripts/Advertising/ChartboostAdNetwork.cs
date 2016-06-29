using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if USE_CHARTBOOST
using ChartboostSDK;
#endif

public static class ChartboostAdNetwork
{
#if USE_CHARTBOOST
    static AdTester.Label log;

	[RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        Chartboost.Create();
		Chartboost.cacheRewardedVideo(CBLocation.Default);
        Chartboost.cacheInterstitial(CBLocation.Default);
    }

    //
    // U s e  b y  A d C e n t r a l
    //
    public static bool PlayAd(string placementName, bool isIncentivized)
    {
    	if (isIncentivized)
    	{
			return PlayRewardedAd();
		}
		else
		{
			return PlayNormalAd();
		}
    }
    
    private static bool PlayNormalAd()
    {
        // Note: We can used named locations.
        // Chartboost.showInterstitial(CBLocation.locationFromName("CustomLocation"));
        // https://answers.chartboost.com/hc/en-us/articles/204888915

        Chartboost.cacheInterstitial(CBLocation.Default);

        if (Chartboost.hasInterstitial(CBLocation.Default))
        {
            Chartboost.showInterstitial(CBLocation.Default);
            Chartboost.cacheInterstitial(CBLocation.Default);

            return true;
        }
        return false;
    }

    private static bool PlayRewardedAd()
    {
        Chartboost.cacheRewardedVideo(CBLocation.Default);

        if (Chartboost.hasRewardedVideo(CBLocation.Default)) 
		{
			Chartboost.showRewardedVideo(CBLocation.Default);
            Chartboost.cacheRewardedVideo(CBLocation.Default);

            return true;
		}
		return false;
	}
#endif

#if UNITY_EDITOR

    // CUSTOM COMPILER DEFINES
    const string UseChartboost = "USE_CHARTBOOST";	// when define is set, we use ads from Chartboost

	// Chartboost menus
	// ----------------

	[MenuItem("Ad Networks/Chartboost/Enable", false, 0)]
    private static void EnableChartboost()
    {
		AdNetworkSettingsHelper.SetEnabled(UseChartboost, true);
    }

	[MenuItem("Ad Networks/Chartboost/Enable", true)]
	private static bool EnableChartboostValidate()
    {
		return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseChartboost) != AdNetworkSettingsHelper.DefinedForPlatforms.All;
    }

    [MenuItem("Ad Networks/Chartboost/Disable", false, 1)]
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

