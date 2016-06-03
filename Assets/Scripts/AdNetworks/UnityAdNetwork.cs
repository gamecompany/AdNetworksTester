using UnityEngine;
using UnityEngine.Advertisements;

public static class UnityAdNetwork
{
	[RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
		if (Advertisement.isSupported) 
		{
			UnityAdsSettings settings = UnityAdsSettings.Instance;

            Debug.LogFormat("Settings: {0}", settings);

			Advertisement.Initialize(settings.AppID, testMode: settings.ShowTestAds);

        }
    }

	public static bool PlayAd(string placementName, bool isIncentivized)
    {
        // we need to translate the placement from what the code calls it to
        // what the ad network knows it as
#if AD_CENTRAL
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

        if (Advertisement.IsReady (zoneID)) 
		{
			Advertisement.Show(zoneID);
			return true;
		}
		return false;
	}

    public static bool PlayAd(int i)
    {
        // Get the zone ID name for the ad
        string zoneID = GetZoneID(i);
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

    public static bool PlayAd(int i, UnityEngine.Events.UnityAction onAdPlay, UnityEngine.Events.UnityAction adNotReady)
    {
        // Get the zone ID name for the ad
        string zoneID = GetZoneID(i);
        // Is it ready to display
        if(Advertisement.IsReady(zoneID))
        {
            // Display
            Advertisement.Show(zoneID);
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
}
