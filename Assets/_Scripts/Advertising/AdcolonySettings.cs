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
        public string Ad5Death;		    // Death Extra Life Popup
        public string Ad6Powerup;       // Daily rewards, power ip

        public string this[int i]
        {
            get
            {
                switch(i)
                {
                    case 0: return Ad1Launch;
                    case 1: return Ad2FreeGame;
                    case 2: return Ad3MoreGame;
                    case 3: return Ad4Trailers;
                    case 4: return Ad5Death;
                    case 5: return Ad6Powerup;
                }

                return null;
            }
        }

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
				instance = AdNetworkSettingsHelper.GetResourceInstance<AdcolonySettings>("_Scripts/Advertising", "Resources", "AdcolonySettings", "asset");
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
	[MenuItem("Ad Networks/AdColony/Edit Settings...", false, 20)]
	public static void Edit()
	{
		Selection.activeObject = Instance;
	}
	#endif
}
