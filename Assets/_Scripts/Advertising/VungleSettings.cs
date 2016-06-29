#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

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
	[MenuItem("Ad Networks/Vungle/Edit Settings...", false, 20)]
    public static void Edit()
    {
        Selection.activeObject = Instance;
    }
#endif
}
