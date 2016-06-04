using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class VungleSettings : ScriptableObject 
{
	public string androidAppId;
	public string iosAppId;
	public string windowsAppId;

	private static VungleSettings instance;
	public static VungleSettings Instance
    {
        get
        {
            if (instance == null)
            {
				instance = AdNetworkSettingsHelper.GetResourceInstance<VungleSettings>("_Scripts/Advertising", "Resources", "VungleSettings", "asset");
			}
			return instance;
        }
    }

#if UNITY_EDITOR
	// These are menu items which appear in the editor

	// CUSTOM COMPILER DEFINES
	const string UseVungle = "USE_VUNGLE";	// when define it set, we use ads from Vungle

	[MenuItem("Ad Networks/Vungle/Edit Settings...", false, 25)]
    public static void Edit()
    {
        Selection.activeObject = Instance;
    }

	// Vungle menus
	// ----------------

	[MenuItem("Ad Networks/Vungle/Enable", false, 26)]
    private static void EnableVungle()
    {
		AdNetworkSettingsHelper.SetEnabled(UseVungle, true);
    }

	[MenuItem("Ad Networks/Vungle/Enable", true)]
	private static bool EnableVungleValidate()
    {
		return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseVungle) != AdNetworkSettingsHelper.DefinedForPlatforms.All;
    }

	[MenuItem("Ad Networks/Vungle/Disable", false, 27)]
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
