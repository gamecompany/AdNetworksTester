using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UnityAdsSettings : ScriptableObject 
{
	public string AppID; 
	public string Ad1Launch; 		// Launch PowerUp Popup
	public string Ad2FreeGame; 		// Free Game Button
	public string Ad3MoreGame; 		// More Game Button
	public string Ad4Trailers; 		// Game Trailer Button
	public string Ad5Death;		    // Death Extra Life Popup
	//public string Ad6PowerUp; 	// PowerUp Popup

	public bool ShowTestAds;

	private static UnityAdsSettings instance;
	public static UnityAdsSettings Instance
    {
        get
        {
            if (instance == null)
            {
				instance = AdNetworkSettingsHelper.GetResourceInstance<UnityAdsSettings>("_Scripts/Advertising", "Resources", "UnityAdsSettings", "asset");
			}
			return instance;
        }
    }

#if UNITY_EDITOR
	// These are menu items which appear in the editor

	// CUSTOM COMPILER DEFINES
	// None.  Unity Ads are always included, and used as our fallback

	[MenuItem("Ad Networks/Unity Ads/Edit Settings...", false, 20)]
    public static void Edit()
    {
        Selection.activeObject = Instance;
    }
#endif
}
