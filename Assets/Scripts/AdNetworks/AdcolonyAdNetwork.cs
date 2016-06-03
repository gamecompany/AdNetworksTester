#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public static class AdcolonyAdNetwork
{
	#if USE_ADCOLONY
	[RuntimeInitializeOnLoadMethod]
	static void Initialize()
	{
		AdcolonySettings settings = AdcolonySettings.Instance;

        /*Debug.LogFormat("Adcolony -> appVersion: {0}, appID: {1}", settings.appVersion, settings.AppID);
        
        foreach(string zone in settings.GetZoneIDs())
        {
            Debug.LogFormat("Adcolony -> zone: {0}", zone);
        }*/

		AdColony.Configure(settings.appVersion, settings.AppID, settings.GetZoneIDs());

	}

	public static bool PlayAd(string placementName, bool isIncentivized)
	{
		AdcolonySettings settings = AdcolonySettings.Instance;

        // we need to translate the placement from what the code calls it to
        // what the ad network knows it as
#if AD_CENTRAL
        int index = AdCentral.IndexOfAdPlacement(placementName);
#else
        int index = 0;
#endif

        string zoneID = GetZoneID(index);
        
        if (null == zoneID)
        {
            Debug.LogErrorFormat("Unknown ad placement \"{0}\"; Unity Ads doesn't know what to show.", placementName);
            return false;
        }

        if (AdColony.IsVideoAvailable (zoneID)) 
		{
			AdColony.ShowVideoAd(zoneID);

            return true;
		}

		return false;
    }

    public static bool PlayAd(int i)
    {
        // Get the zone ID name for the ad
        string zoneID = GetZoneID(i);
        // Is it ready to display
        if (AdColony.IsVideoAvailable(zoneID))
        {
            // Display
            AdColony.ShowVideoAd(zoneID);
            // success
            return true;
        }
        // Couldn't display
        return false;
    }

    public static bool PlayAd(int i, UnityEngine.Events.UnityAction onAdPlay, UnityEngine.Events.UnityAction adNotReady)
    {
        // Get the zone ID name for the ad
        string zoneID = GetZoneID(i);
        // Is it ready to display
        if (AdColony.IsVideoAvailable(zoneID))
        {
            // Display
            AdColony.ShowVideoAd(zoneID);
            // Callback
            if (null != onAdPlay) onAdPlay.Invoke();
            // success
            return true;
        }
        // Ad is not ready, callback
        if (null != adNotReady) adNotReady.Invoke();
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
    const string UseAdcolony = "USE_ADCOLONY";	// when define is set, we use ads from Ad Colony

	// Ad Colony menus
	// ----------------

	[MenuItem("Ad Networks/AdColony/Enable", false, 2)]
	private static void EnableAdColony()
	{
		AdNetworkSettingsHelper.SetEnabled(UseAdcolony, true);
	}

	[MenuItem("Ad Networks/AdColony/Enable", true)]
	private static bool EnableAdColonyValidate()
	{
		return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseAdcolony) != AdNetworkSettingsHelper.DefinedForPlatforms.All;
	}

	[MenuItem("Ad Networks/AdColony/Disable", false, 3)]
	private static void DisableAdColony()
	{
		AdNetworkSettingsHelper.SetEnabled(UseAdcolony, false);
	}

	[MenuItem("Ad Networks/AdColony/Disable", true)]
	private static bool DisableAdColonyValidate()
	{
		return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseAdcolony) != AdNetworkSettingsHelper.DefinedForPlatforms.None;
	}
	#endif
}

