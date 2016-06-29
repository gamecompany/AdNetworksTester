#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

#if USE_UNITYADS
using UnityEngine.Advertisements;
#endif

public static class UnityAdNetwork
{
#if USE_UNITYADS
    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
		if (Advertisement.isSupported) 
		{
			UnityAdsSettings settings = UnityAdsSettings.Instance;

			Advertisement.Initialize(settings.AppID, testMode: settings.ShowTestAds);
        }
    }

	public static bool PlayAd(string placementName, bool isIncentivized)
    {
        // we need to translate the placement from what the code calls it to
        // what the ad network knows it as
#if !NOT_AD_CENTRAL
        int index = AdCentral.IndexOfAdPlacement(placementName);
#else
        int index = 0;
#endif
        // Get the ad zone name
        string zoneID = GetZoneID(index);

        if(null == zoneID)
        {
            Debug.LogErrorFormat("Unknown ad placement \"{0}\"; Unity Ads doesn't know what to show.", placementName);
            return false;
        }

        return PlayNormalAd(zoneID);
    }

    public static bool PlayNormalAd(string zoneID)
    {
        // Is it ready to display
        if (Advertisement.IsReady(zoneID))
        {
            // Display
            Advertisement.Show(zoneID);
            // success
            return true;
        }
        // Couldn't display
        return false;
    }
    
    public static string GetZoneID(int index)
    {
        UnityAdsSettings settings = UnityAdsSettings.Instance;

        switch (index + 1)
        {
            case 1: return settings.Ad1Launch;
            case 2: return settings.Ad2FreeGame;
            case 3: return settings.Ad3MoreGame;
            case 4: return settings.Ad4Trailers;
            case 5: // Fall through
            case 6: return settings.Ad5Death;
        }

        return null;
    }
#endif

#if UNITY_EDITOR

    // CUSTOM COMPILER DEFINES
    const string UseUnityAds = "USE_UNITYADS";  // when define it set, we use ads from Heyzap

    // Heyzap menus
    // ----------------

    [MenuItem("Ad Networks/Unity Ads/Enable", false, 0)]
    private static void EnableUnityAds()
    {
        AdNetworkSettingsHelper.SetEnabled(UseUnityAds, true);
    }

    [MenuItem("Ad Networks/Unity Ads/Enable", true)]
    private static bool EnableUnityAdsValidate()
    {
        return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseUnityAds) != AdNetworkSettingsHelper.DefinedForPlatforms.All;
    }

    [MenuItem("Ad Networks/Unity Ads/Disable", false, 1)]
    private static void DisableUnityAdsValidate()
    {
        AdNetworkSettingsHelper.SetEnabled(UseUnityAds, false);
    }

    [MenuItem("Ad Networks/Unity Ads/Disable", true)]
    private static bool DisableUnityAds()
    {
        return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseUnityAds) != AdNetworkSettingsHelper.DefinedForPlatforms.None;
    }
#endif
}
