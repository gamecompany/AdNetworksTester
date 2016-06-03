using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


// Utility methods to help out with turning different types of ads on and set them up.
// Contains a utility to get custom resources (adapted from ChartboostSDK.CBSettings)
// Contains code to turn on/off compile-time constants,
// based on code for UnityStandardAssets.CrossPlatformInput.Inspector.CrossPlatformInitialize
public static class AdNetworkSettingsHelper 
{
	// Returns an object with settings, creating it if run in the editor
	// If your file is supposed to go in /path/to/myUnityProject/Assets/folder1/folder2/folder3/myresource.ext,
	// then pass in "folder1/folder2", "folder3", "myresource", "ext"
	public static T GetResourceInstance<T>
		(string settingsPathExceptFinalFolder, 
		 string settingsFinalFolder, 
		 string resourceName, 
		 string extension)
		 where T : ScriptableObject
	{
		T instance = Resources.Load(resourceName) as T;
		if (instance == null)
		{
			// The object couldn't be found. Create it.
			instance = ScriptableObject.CreateInstance<T>();
			#if UNITY_EDITOR
				string properPath = Path.Combine(Path.Combine(Application.dataPath, settingsPathExceptFinalFolder), settingsFinalFolder);
		        if (!Directory.Exists(properPath))
		        {
		        	string basePath = Path.Combine("Assets", settingsPathExceptFinalFolder);
		            AssetDatabase.CreateFolder(basePath, settingsFinalFolder);
		        }

		        string fullPath = Path.Combine(
		        					Path.Combine(
		        						Path.Combine("Assets", settingsPathExceptFinalFolder), 
		        						settingsFinalFolder),
		        					resourceName + "." + extension);
		        AssetDatabase.CreateAsset(instance, fullPath);
		    #endif
		}
		return instance;
	}

#if UNITY_EDITOR
	// CUSTOM COMPILER DEFINES
	//
	//const string UseChartboost = "USE_CHARTBOOST";	// when define is set, we use ads from Chartboost
	//const string UseVungle = "USE_VUNGLE";	// when define is set, we use ads from Vungle
	//const string UseHeyzap = "USE_HEYZAP";	// when define it set, we use ads from Heyzap
	// There is no define for Unity Ads; they are always our fallback

	// Let's us tell if a given definition is defined for all platforms or not
	public enum DefinedForPlatforms
	{
		None,
		Some,
		All
	}

    // These are the build targets for which we'll change environment variables
	private static BuildTargetGroup[] buildTargetGroups = new BuildTargetGroup[]
	{	
	//	BuildTargetGroup.Standalone,
	//	BuildTargetGroup.WebPlayer,
        BuildTargetGroup.Android,
        BuildTargetGroup.iOS,
    //  BuildTargetGroup.WP8,
    //  BuildTargetGroup.BlackBerry,
	//	BuildTargetGroup.PSM, 
	//	BuildTargetGroup.Tizen, 
	//	BuildTargetGroup.WSA 
    };

    // Here we add or remove environment variables, as requested
	public static void SetEnabled(string defineName, bool enable, BuildTargetGroup[] targets = null)
    {
		if (targets == null)
		{
			targets = buildTargetGroups;
		}

        foreach (var platform in targets)
        {
            var defines = GetDefinesList(platform);

            // If the platform's definitions don't match what we want them to be
            if (enable != defines.Contains(defineName))
            {
            	// then add or remove the definition, as requested
            	if (enable)
            	{
            		defines.Add(defineName);
            	}
            	else
            	{
            		defines.Remove(defineName);
            	}

				string definesString = string.Join(";", defines.ToArray());
	            PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, definesString);
            }
        }
    }

    // Returns a list of all the defined variables for a given group
    public static List<string> GetDefinesList(BuildTargetGroup group)
    {
        return new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';'));
    }

    // Looks through the definitions in all the requested platforms, 
    // and tells you if they all have the requested definition or not
	public static DefinedForPlatforms IsDefinedForPlatforms(string define, BuildTargetGroup[] targets = null)
	{
		int set_count = 0;
		int unset_count = 0;

		if (targets == null)
		{
			targets = buildTargetGroups;
		}

		for (int i = 0; i < targets.Length; ++i)
		{
			if (GetDefinesList(targets[i]).Contains(define))
			{
				set_count++;
			}
			else
			{
				unset_count++;
			}
		}

		if (unset_count == 0)
		{
			return DefinedForPlatforms.All;
		}
		else if (set_count == 0)
		{
			return DefinedForPlatforms.None;
		}
		else
		{
			return DefinedForPlatforms.Some;
		}
	}

	public static List<string> GetDefinesList()
    {
        return GetDefinesList(buildTargetGroups[0]);
    }
#endif
}

