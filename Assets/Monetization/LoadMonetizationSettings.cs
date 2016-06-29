using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LoadMonetizationSettings : MonoBehaviour 
{
	// Use this for initialization
	IEnumerator Start () 
	{
		DontDestroyOnLoad(this);

#if DEBUG_ADVERTISING
		AdCentral.logoSceneStartTime = Time.unscaledTime;
		AdCentral.settingsDownloadStartTime = Time.unscaledTime;
#endif

        WWW downloadedJson = new WWW(AdCentral.AdSettingsUrl);
		yield return downloadedJson;

#if DEBUG_ADVERTISING
		AdCentral.settingsDownloadEndTime = Time.unscaledTime;
		AdCentral.settingsDownloadedSuccessfully = (downloadedJson.error == null);
#endif

		if (AdCentral.LoadJsonAdSlots(AdCentral.JsonSource.Downloaded, downloadedJson.text))
		{
			// if the data parsed successfully, we'll cache it
#if UNITY_EDITOR
				File.WriteAllText("Assets/Monetization/Resources/" + AdCentral.AdSettingsFileName + ".json", downloadedJson.text);
				AssetDatabase.Refresh();
#else
				// Record this data to a new file on the device; we'll try reading from it first
				string _toWrite = AdCentral.PersistentAdSettingsPath;
				File.WriteAllText(_toWrite, downloadedJson.text);
#endif
        }

#if !DEBUG_ADVERTISING
		// Get rid of this object ... unless we are using it to time level loading
		DestroyObject(this.gameObject);
#endif
    }

#if DEBUG_ADVERTISING
	void OnLevelWasLoaded(int level)
	{
		Debug.LogWarningFormat("Level {0} loaded at {1}", level, Time.unscaledTime);

		if (level == 1 && AdCentral.menuSceneStartTime == 0)
		{
			AdCentral.menuSceneStartTime = Time.unscaledTime;
			AdCentral.UpdateLoadingTimeOutput();
		}
	}
#endif

}
