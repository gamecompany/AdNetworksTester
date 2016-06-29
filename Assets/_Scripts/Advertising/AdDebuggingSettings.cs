using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AdDebuggingSettings : ScriptableObject 
{
	public GameObject diagnosticCanvasPrefab;
	public GameObject diagnosticLinePrefab;
	public GameObject diagnosticTextOutputLinePrefab;

	private static AdDebuggingSettings instance;
	public static AdDebuggingSettings Instance
    {
        get
        {
            if (instance == null)
            {
				instance = AdNetworkSettingsHelper.GetResourceInstance<AdDebuggingSettings>("_Scripts/Advertising", "Resources", "AdDebuggingSettings", "asset");
			}
			return instance;
        }
    }

#if UNITY_EDITOR
	// These are menu items which appear in the editor

	// CUSTOM COMPILER DEFINES
	const string UseAdDiagnostics = "DEBUG_ADVERTISING";	// when define it set, we show diagnostics for the ads

	[MenuItem("Ad Networks/Diagnostics/Edit Settings...", false, 105)]
    public static void Edit()
    {
        Selection.activeObject = Instance;
    }

	// AdDiagnostics menus
	// ----------------

	[MenuItem("Ad Networks/Diagnostics/Enable", false, 106)]
    private static void EnableAdDiagnostics()
    {
		AdNetworkSettingsHelper.SetEnabled(UseAdDiagnostics, true);
    }

	[MenuItem("Ad Networks/Diagnostics/Enable", true)]
	private static bool EnableAdDiagnosticsValidate()
    {
		return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseAdDiagnostics) != AdNetworkSettingsHelper.DefinedForPlatforms.All;
    }

	[MenuItem("Ad Networks/Diagnostics/Disable", false, 107)]
    private static void DisableAdDiagnostics()
    {
		AdNetworkSettingsHelper.SetEnabled(UseAdDiagnostics, false);
    }

	[MenuItem("Ad Networks/Diagnostics/Disable", true)]
	private static bool DisableAdDiagnosticsValidate()
    {
		return AdNetworkSettingsHelper.IsDefinedForPlatforms(UseAdDiagnostics) != AdNetworkSettingsHelper.DefinedForPlatforms.None;
    }
#endif
}
