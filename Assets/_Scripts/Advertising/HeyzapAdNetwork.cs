using UnityEngine;
using System.Collections;
#if USE_HEYZAP
using Heyzap;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HeyzapAdNetwork : MonoBehaviour 
{
#if USE_HEYZAP

	[RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
    	string HeyzapID;

    	switch(StudioSettings.Instance.studio)
    	{
			case StudioSettings.Studio.TopGame:
				HeyzapID="cde2146866f96c9ad90736c1af512e13";
				break;
			case StudioSettings.Studio.FreeGame:
				HeyzapID="ef2a0c12b6d378ddce5063ca08a660b9";
				break;
			case StudioSettings.Studio.CrystalBuffalo:
				HeyzapID="70a2f87d4c05ba041dada7f272651dba";
				break;
			case StudioSettings.Studio.SurvivalGame:
				HeyzapID="9dff8b4f221dbad7cbbeeb52b2fc6125";
				break;
			default:
				HeyzapID="cde2146866f96c9ad90736c1af512e13";
				break;
		}

		// TO DO: Something needs to be done to initialize Heyzap!
		// It is no longer in the launch controller.
		HeyzapAds.Start(HeyzapID,HeyzapAds.FLAG_NO_OPTIONS);
        HZVideoAd.Fetch();
        HZIncentivizedAd.Fetch();
    }

	public static bool PlayAd(string placementName, bool isIncentivized)
    {
		if(isIncentivized)
		{
			return PlayHeyzapIncentivizedAd();
		}
		else
		{
			return PlayHeyzapVideoAd();
		}
    }

    public static bool PlayAd()
    {
        // fetch the video
        HZVideoAd.Fetch();
        // Check for availability
        if (HZVideoAd.IsAvailable())
        {
            // Show the ad
            HZVideoAd.Show();
            // E x i t
            return true;
        }
        
        return false;
    }

    public static bool PlayAd(UnityEngine.Events.UnityAction onAdPlay, UnityEngine.Events.UnityAction adNotAviable)
    {
        // fetch the video
        HZVideoAd.Fetch();
        // Check for availability
        if (HZVideoAd.IsAvailable())
        {
            // Show the ad
            HZVideoAd.Show();
            // Callback
            if(null != onAdPlay) onAdPlay.Invoke();
            // E x i t
            return true;
        }

        // Failed debug it
        if (null != adNotAviable) adNotAviable.Invoke();

        return false;
    }

    // Untouched code copied from AdCentral
    // ------------

	private static bool PlayHeyzapVideoAd()
	{
		if (HZVideoAd.IsAvailable())
		{
			HZVideoAd.Show();
			HZVideoAd.Fetch();
			return true;
		}
		return false;
	}

	private static bool PlayHeyzapIncentivizedAd()
	{
		if(HZIncentivizedAd.IsAvailable())
		{
			HZIncentivizedAd.Show();
			HZIncentivizedAd.Fetch();
			return true;
		}
		return false;
	}
#endif


#if UNITY_EDITOR

	// CUSTOM COMPILER DEFINES
	const string UseHeyzap = "USE_HEYZAP";	// when define it set, we use ads from Heyzap

	// Heyzap menus
	// ----------------

	[MenuItem("Ad Networks/Heyzap/Enable", false, 2)]
    private static void EnableHeyzap()
    {
		AdNetworkSettingsHelper.SetEnabled(UseHeyzap, true);
    }

	[MenuItem("Ad Networks/Heyzap/Enable", true)]
	private static bool EnableHeyzapValidate()
    {
		return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseHeyzap) != AdNetworkSettingsHelper.DefinedForPlatforms.All;
    }

	[MenuItem("Ad Networks/Heyzap/Disable", true)]
	private static bool DisableHeyzapValidate()
	{
		return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseHeyzap) != AdNetworkSettingsHelper.DefinedForPlatforms.None;
	}

	[MenuItem("Ad Networks/Heyzap/Disable", false, 3)]
    private static void DisableHeyzap()
    {
		AdNetworkSettingsHelper.SetEnabled(UseHeyzap, false);
    }

#endif
}