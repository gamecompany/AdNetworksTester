#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

#if USE_VUNGLE
public static class VungleAdNetwork
{

    // Create options
    static Dictionary<string, object> options = new Dictionary<string, object>();

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
    	VungleSettings settings = VungleSettings.Instance;
		//Your App IDs can be found in the Vungle Dashboard on your apps' pages
		Vungle.init(settings.androidAppId, settings.iosAppId, settings.windowsAppId);
    }

    //
    // U s e  b y  A d C e n t r a l
    //

    public static bool PlayAd(string placementName, bool isIncentivized)
    {
		if (Vungle.isAdvertAvailable())
        {
            options.Clear();
            // Set incentivized flag
            options["incentivized"] = isIncentivized;
            // Play ad
            Vungle.playAdWithOptions(options);
			return true;
		}
		return false;
    }
#endif

#if UNITY_EDITOR

    // CUSTOM COMPILER DEFINES
    const string UseVungle = "USE_VUNGLE";	// when define it set, we use ads from Vungle


    // Vungle menus
    // ----------------

    [MenuItem("Ad Networks/Vungle/Enable", false, 0)]
    private static void EnableVungle()
    {
        AdNetworkSettingsHelper.SetEnabled(UseVungle, true);
    }

    [MenuItem("Ad Networks/Vungle/Enable", true)]
    private static bool EnableVungleValidate()
    {
        return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseVungle) != AdNetworkSettingsHelper.DefinedForPlatforms.All;
    }

    [MenuItem("Ad Networks/Vungle/Disable", false, 1)]
    private static void DisableVungle()
    {
        AdNetworkSettingsHelper.SetEnabled(UseVungle, false);
    }

    [MenuItem("Ad Networks/Vungle/Disable", true)]
    private static bool DisableVungleValidate()
    {
        return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseVungle) != AdNetworkSettingsHelper.DefinedForPlatforms.None;
    }
#endif
}
