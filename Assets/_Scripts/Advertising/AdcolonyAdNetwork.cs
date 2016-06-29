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
        
		AdColony.Configure(settings.appVersion, settings.AppID, settings.GetZoneIDs());
	}

    //
    // U s e  b y  A d C e n t r a l
    //

	public static bool PlayAd(string placementName, bool isIncentivized)
	{
        // we need to translate the placement from what the code calls it to
        // what the ad network knows it as
#if !NOT_AD_CENTRAL
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

        if(isIncentivized)
        {
            return PlayRewardedAd(zoneID);
        }

        return PlayNormalAd(zoneID);
    }


    public static bool PlayNormalAd(string zoneID)
    {
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


    public static bool PlayRewardedAd(string zoneID)
    {
        // http://support.adcolony.com/customer/en/portal/articles/313648-value-exchange-v4vc%E2%84%A2-#s1q3

#if USING_V4VC
        // Is it ready to display
        if (AdColony.IsV4VCAvailable(zoneID))
        {
            // Display
            AdColony.ShowV4VC(true, zoneID);
            // success
            return true;
        }
        // Couldn't display
        return false;
#else
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
#endif
    }
#endif

    public static string GetZoneID(int index)
    {
        AdcolonySettings settings = AdcolonySettings.Instance;

        return settings.zoneID[index];
    }

#if UNITY_EDITOR

    // CUSTOM COMPILER DEFINES
    const string UseAdcolony = "USE_ADCOLONY";	// when define is set, we use ads from Ad Colony

	// Ad Colony menus
	// ----------------

	[MenuItem("Ad Networks/AdColony/Enable", false, 0)]
	private static void EnableAdColony()
	{
		AdNetworkSettingsHelper.SetEnabled(UseAdcolony, true);
	}

	[MenuItem("Ad Networks/AdColony/Enable", true)]
	private static bool EnableAdColonyValidate()
	{
		return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseAdcolony) != AdNetworkSettingsHelper.DefinedForPlatforms.All;
	}

	[MenuItem("Ad Networks/AdColony/Disable", false, 1)]
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

