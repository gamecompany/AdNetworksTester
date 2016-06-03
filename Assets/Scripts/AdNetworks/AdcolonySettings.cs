using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AdcolonySettings : ScriptableObject 
{
    [System.Serializable]
    public struct ZoneIDs
    {
        public string Ad1Launch;        // Launch PowerUp Popup
        public string Ad2FreeGame;      // Free Game Button
        public string Ad3MoreGame;      // More Game Button
        public string Ad4Trailers;      // Game Trailer Button
        public string Ad5Death;		// Death Extra Life Popup

        public string[] GetAll()
        {
            return new string[] { Ad1Launch, Ad2FreeGame, Ad3MoreGame, Ad4Trailers, Ad5Death };
        }
    }

    //
    // V a r i a b l e s
    //

    private static AdcolonySettings instance;

    public string appVersion = "Version:1.0,store:google";
    public string AppID = "app4a74095801a64b1098";
    public ZoneIDs zoneID;

    //
    // P r o p e r t i e s
    //

    public static AdcolonySettings Instance
	{
		get
		{
			if (instance == null)
			{
				instance = AdNetworkSettingsHelper.GetResourceInstance<AdcolonySettings>("Scripts/AdNetworks", "Resources", "AdcolonySettings", "asset");
			}
			return instance;
		}
	}
    
    //
    // U s e r
    //

    public string[] GetZoneIDs()
    {
        return zoneID.GetAll();
    }

	#if UNITY_EDITOR
	// These are menu items which appear in the editor

	// CUSTOM COMPILER DEFINES
	const string UseAdcolony = "USE_ADCOLONY";	// when define it set, we use ads from Vungle

	[MenuItem("Ad Networks/AdColony/Edit Settings...", false, 1)]
	public static void Edit()
	{
		Selection.activeObject = Instance;
	}

	// Vungle menus
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
