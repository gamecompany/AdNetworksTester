using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StudioSettings : ScriptableObject 
{
	public enum Studio
	{
		TopGame,
		FreeGame,
		CrystalBuffalo,
		SurvivalGame
	}
	public Studio studio = Studio.TopGame;

	private static StudioSettings instance;
	public static StudioSettings Instance
    {
        get
        {
            if (instance == null)
            {
				instance = AdNetworkSettingsHelper.GetResourceInstance<StudioSettings>("_Scripts/Advertising", "Resources", "StudioSettings", "asset");
			}
			return instance;
        }
    }

	public string Email
	{
		get
		{
			switch (studio)
			{
				case Studio.TopGame:
					return "Gamecompanya@gmail.com";
				case Studio.FreeGame:
					return "Gamecompanyb@gmail.com";
				case Studio.CrystalBuffalo:
					return "Gamecompanyc@gmail.com";
				case Studio.SurvivalGame:
					return "Gamecompanye@gmail.com";
				default:
					return "Gamecompanya@gmail.com";
			}
		}
	}

#if UNITY_EDITOR
	// These are menu items which appear in the editor
	[MenuItem("Ad Networks/Edit Game Studio...", false, 100)]
    public static void Edit()
    {
        Selection.activeObject = Instance;
    }
#endif
}
