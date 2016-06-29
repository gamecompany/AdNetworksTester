using UnityEngine;

#if USE_HEYZAP
using Heyzap;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HeyzapAdNetwork : MonoBehaviour 
{
#if USE_HEYZAP
    
    static AdTester.Label log;
    static string publisherID;

    static string PublisherID
    {
        get
        {
            if (!StudioSettings.Instance) publisherID = "";

            switch (StudioSettings.Instance.studio)
            {
                case StudioSettings.Studio.TopGame:
                    publisherID = "cde2146866f96c9ad90736c1af512e13";
                    break;
                case StudioSettings.Studio.FreeGame:
                    publisherID = "ef2a0c12b6d378ddce5063ca08a660b9";
                    break;
                case StudioSettings.Studio.CrystalBuffalo:
                    publisherID = "70a2f87d4c05ba041dada7f272651dba";
                    break;
                case StudioSettings.Studio.SurvivalGame:
                    publisherID = "9dff8b4f221dbad7cbbeeb52b2fc6125";
                    break;
                default:
                    publisherID = "cde2146866f96c9ad90736c1af512e13";
                    break;
            }

            return publisherID;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        HeyzapAds.Start(PublisherID, HeyzapAds.FLAG_NO_OPTIONS);
        // E x i t
        if (!AdTester.Exists) return;
        // Assign debug log object
        log = AdTester.I["Heyzap"];
        // E x i t
        if (null == log) return;
        // Update label
        log.Update("Is Initialize - "+ HeyzapAds.IsNetworkInitialized(PublisherID));
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

    public static bool PlayNormalAd()
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

    public static bool PlayRewardedAd()
    {
        // fetch the video
        HZIncentivizedAd.Fetch();
        // Check for availability
        if (HZIncentivizedAd.IsAvailable())
        {
            // Show the ad
            HZIncentivizedAd.Show();
            // E x i t
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

	[MenuItem("Ad Networks/Heyzap/Enable", false, 0)]
    private static void EnableHeyzap()
    {
		AdNetworkSettingsHelper.SetEnabled(UseHeyzap, true);
    }

	[MenuItem("Ad Networks/Heyzap/Enable", true)]
	private static bool EnableHeyzapValidate()
    {
		return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseHeyzap) != AdNetworkSettingsHelper.DefinedForPlatforms.All;
    }

    [MenuItem("Ad Networks/Heyzap/Disable", false, 1)]
    private static void DisableHeyzap()
    {
        AdNetworkSettingsHelper.SetEnabled(UseHeyzap, false);
    }

    [MenuItem("Ad Networks/Heyzap/Disable", true)]
	private static bool DisableHeyzapValidate()
	{
		return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseHeyzap) != AdNetworkSettingsHelper.DefinedForPlatforms.None;
	}
#endif
}