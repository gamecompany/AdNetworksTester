using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class AdCentral
{
    delegate bool AdFunction(string placementName, bool incentivized);

    private static JsonSource m_JsonSource;
    private static string[] m_AdSlotName;
    private static AdFunction[] m_AdSlotFunction;

    class AdSlotDetails
    {
        public string[] slotName;
        public AdFunction[] slotFunction;
        public bool validResult;
    };

    public static bool m_StackCreated = false;
    private static int m_StackIndex = 0;

    public static bool m_SaveLife = false;
    public static string adName;

    public const string AdSettingsFileName = "ten-slot-settings";

    public static string AdSettingsUrl = string.Format("http://topgamestudio.com/DynamicVariables/{0}/ten-slot-settings.json", bundleId);

    public enum AdPlacementID
    {
        Ad1Launch,
        Ad2FreeGame,
        Ad3MoreGame,
        Ad4Trailers,
        Ad5Death,
        Ad6PowerUp
    }

    public static string bundleId
    {
    	get 
    	{
			#if UNITY_ANDROID || UNITY_IOS
	            #if UNITY_EDITOR
    	            return PlayerSettings.bundleIdentifier;
        	    #else 
            	    return Application.bundleIdentifier;
	            #endif
    	    #else
        	    #error Bundle Identifier is not defined for this platform.
        	#endif
    	}
    }

    public static string PersistentAdSettingsPath
    {
        get
        {
            return Application.persistentDataPath + "/ten-slot-settings.json";
        }
    }

    public static string[] AdPlacements = new string[]{
		"Ad1Launch",
		"Ad2FreeGame",
		"Ad3MoreGame",
		"Ad4Trailers",
		"Ad5Death",
		"Ad6PowerUp"
	};

	// These match the ads above
	public static bool[] AdIsIncentivized = new bool[]{
		true,	// Ad1Launch is used when you get a powerup
		false,
		false,
		false,
		true,
		true
	};

	// These are the sources for where we get our ad slot settings
	// They are in order from the least desirable to the most desirable to use
	public enum JsonSource
	{
		Fallback,	// fail safe information -- shows Unity Ads
		Asset,		// asset included with the app when it shipped
		File,		// data written last time we successfully downloaded JSON from the internet
		Downloaded  // data just downloaded from the internet
	};

	const int maxNumberOfSlots = 20;

	private static int StackIndex {
		get {
			return m_StackIndex;
		}
		set {
			m_StackIndex = value;
			if (m_StackIndex >= m_AdSlotName.Length || m_StackIndex < 0) 
			{
				m_StackIndex = 0;
			}
		}
	}

#if DEBUG_ADVERTISING
	private static GameObject debugCanvas;
	private static AdDebugLine[] debugLine;
	private static AdDebugLine highlightedLine;
	private static Text loadingTimeOutput;

	public static double logoSceneStartTime;
	public static double menuSceneStartTime;
	public static double settingsDownloadStartTime;
	public static double settingsDownloadEndTime;
	public static bool settingsDownloadedSuccessfully;
#endif


	[RuntimeInitializeOnLoadMethod]
    static void InitializeAdCentral()
    {
    	// Load the JSON source multiple times
    	// Each valid data source will overwrite the previous results
    	LoadJsonAdSlots(JsonSource.Fallback);
    	LoadJsonAdSlots(JsonSource.Asset);
    	LoadJsonAdSlots(JsonSource.File);

    	// Note: the file will be downloaded from the internet a few seconds after this runs
    }

	// We have ten 'slots' of advertisers we want to use in a given game.
	// This function parses that list
	public static void CreateStack()
	{
		if (m_AdSlotFunction == null)
		{
			Debug.Log("Building list of ad slots.");
		}
		else
		{
			Debug.Log("Rebuilding list of ad slots (with newly-downloaded data)");
		}

		string _jsonText;

		#if UNITY_EDITOR	
			_jsonText = Resources.Load<TextAsset>(AdCentral.AdSettingsFileName).text;
#else
			// If we've download a newer version of the data file, read it
			if(File.Exists(AdCentral.PersistentAdSettingsPath))
			{
				_jsonText = File.ReadAllText(AdCentral.PersistentAdSettingsPath);
			}
			else
			{
				// No updated data file; load the original that shipped with the game
				_jsonText = Resources.Load<TextAsset>(AdCentral.AdSettingsFileName).text;
			}
#endif

        var _jsonDictionary = MiniJSON2.Json.Deserialize(_jsonText) as Dictionary<string,object>;
		int _indexing = 0;
		int slots = _jsonDictionary.Keys.Count;
		m_AdSlotName = new string[slots];
		m_AdSlotFunction = new AdFunction[slots];

		// test with bad JSON file!

		foreach(var keys in _jsonDictionary.Keys)
		{
			var _jsonDict2 = MiniJSON2.Json.Deserialize(MiniJSON2.Json.Serialize(_jsonDictionary[keys])) as Dictionary<string,object>;
			foreach(var key2 in _jsonDict2)
			{
				string slotName = (string)key2.Value;
				m_AdSlotName[_indexing] = slotName;
				m_AdSlotFunction[_indexing] = AdSourceFromString(slotName);
				_indexing++;
			}
		}

		StackIndex = StackIndex;	// make sure index is within bounds (as the array size may've changed)


	}

	// Takes a source, loads the JSON text, and has it parsed
	public static bool LoadJsonAdSlots(JsonSource source, string jsonText = null)
	{
		switch (source)
		{
			// If we have no other data, we fall back to using a stack of Unity Ads
			case JsonSource.Fallback:
				jsonText = "{\"StackPos1\":{\"Stack Order\":\"Unity\"},\"StackPos2\":{\"Stack Order\":\"Unity\"},\"StackPos3\":{\"Stack Order\":\"Unity\"},\"StackPos4\":{\"Stack Order\":\"Unity\"},\"StackPos5\":{\"Stack Order\":\"Unity\"}}";
				break;

			// If we have nothing newer, we use the asset built into the game when it was compiled
			case JsonSource.Asset:
				var resource = Resources.Load<TextAsset>(AdCentral.AdSettingsFileName);
				if (resource != null)
				{
					jsonText = resource.text;
				}
				break;

			// If possible, we load the data from a recently-downloaded file
			case JsonSource.File:
                if (File.Exists(AdCentral.PersistentAdSettingsPath))
				{
					jsonText = File.ReadAllText(AdCentral.PersistentAdSettingsPath);
				}
				break;

            // Json text is already set
            case JsonSource.Downloaded:
                break;
        }

        //Debug.LogFormat("Loaded Json from: {0} -> text: {1}", source, jsonText);

        bool result = SetJsonAdSlots(source, jsonText);

        //Debug.LogFormat("<color=red>Loaded Json from: {0} -> {1}, text: {2}</color>", source, result, jsonText);

        return result;
	}

	// Parses JSON file and sets our ads using it if the file was any good
	private static bool SetJsonAdSlots(JsonSource source, string jsonText)
    {
        // Do we actually have some json text?
        if (string.IsNullOrEmpty(jsonText))
        {
            //Debug.LogWarningFormat("Json source: {0} -> Json text is null or empty", source);

            return false;
        }

        AdSlotDetails details = ParseJsonAdSlots(jsonText);
        
		if (details.validResult)
		{
		    m_AdSlotFunction = details.slotFunction;
		    m_AdSlotName = details.slotName;
		    StackIndex = StackIndex;    // the setter clamps the value to the bounds of the slot function arrays
            m_JsonSource = source;
            UpdateAdSlotsUI();
		}
        
		return details.validResult;
	}

	// Takes data in JSON format and parses it. Returns true if it had reasonable data.
	private static AdSlotDetails ParseJsonAdSlots(string jsonText)
	{
		int maxSlotLength = maxNumberOfSlots;

        // A result with the details
		AdSlotDetails details = new AdSlotDetails();
        // We have not validate the result
		details.validResult = false;

		if (string.IsNullOrEmpty(jsonText))
        {
            return details;
		}

        // Dictionary from json text
        Dictionary<string, object> jsonDictionary;

        try
        {   // Try to deserialize json text
            jsonDictionary = MiniJSON2.Json.Deserialize(jsonText) as Dictionary<string, object>;
        }
        catch(Exception e)
        {
            Debug.LogWarningFormat("Malformed Json error reading advertisment Json: {0}", e);

            return details;
        }

		if (jsonDictionary == null)
        {
            return details;
		}

        // The dictionary inside our deserialize json
        Dictionary<string, object> stackPosDic;
        // Initialize the lists to store details info
        List<string> slotName = new List<string>();
        List<AdFunction> slotFunc = new List<AdFunction>();
        // the current stack pos and stack order name
        string stackPosKey = "";
        string stackOrderKey = "";

        for(int i = 0; i < maxSlotLength; i++)
        {
            stackPosKey = string.Format("StackPos{0}", i + 1);

            try
            {
                stackPosDic = jsonDictionary[stackPosKey] as Dictionary<string, object>;
                stackOrderKey = stackPosDic["Stack Order"] as string;

                if(!string.IsNullOrEmpty(stackOrderKey))
                {
                    // Name of the ad network
                    slotName.Add(stackOrderKey);
                    // Get ad function from the ad network name
                    slotFunc.Add(AdSourceFromString(stackOrderKey));
                }
            }
            catch { }
        }

        if(0 == slotName.Count || 0 == slotFunc.Count)
        {
            Debug.LogWarning("We couldn't find any ad-network specified in the advertisment Json source");
        }

        // Set the arrays of details from our list
        try
        {
            // Try to parse the lists
            details.slotName = slotName.ToArray();
            details.slotFunction = slotFunc.ToArray();
            // Set the valid result from the length of the arrays
            details.validResult = 
            	details.slotName.Length == details.slotFunction.Length &&
            	details.slotName.Length > 0;
        }
        catch { }

		return details;
	}

    public static bool ValidIndex(int length, int index)
    {
        return index >= 0 && index < length;
    }

    public static bool ValidName(string slotName, int i)
    {
        return slotName == "StackPos" + i;
    }

	// If we are debugging the ad slots, this updates the user interface
	public static void UpdateAdSlotsUI()
	{
#if DEBUG_ADVERTISING
			int slots = m_AdSlotName.Length;

			// Create a grid of information so we can see what is going on

			AdDebuggingSettings settings = AdDebuggingSettings.Instance;

			// Start by creating a canvas
			if (debugCanvas == null)
			{
				debugCanvas = GameObject.Instantiate(settings.diagnosticCanvasPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				debugCanvas.name = "Ad Diagnostic Canvas";
				GameObject.DontDestroyOnLoad(debugCanvas);
			}

			// Get the item that'll be the parent of all others here
			Transform lineParent = debugCanvas.transform.GetChild(0);

			int childCount = lineParent.transform.childCount;
			// Delete any children from the previous time this function ran
			for (int i = childCount; i > 0; --i)
			{
				GameObject child = lineParent.transform.GetChild(i-1).gameObject;
				child.SetActive(false);
				GameObject.Destroy(child);
			}


			// Create a list of lines that output debugging information
			debugLine = new AdDebugLine[slots];

			AdDebugLine header = CreateDebugLine<AdDebugLine>(lineParent, settings.diagnosticLinePrefab, "Table header");
			header.MakeHeader();

			for (int i = 0; i < slots; ++i)
			{
				debugLine[i] = CreateDebugLine<AdDebugLine>(lineParent, settings.diagnosticLinePrefab, string.Format("{0} {1}", "Ad Diagnostic Line", i + 1));
				debugLine[i].SetProvider(m_AdSlotName[i]);
			}

			// Finally, create a line for text output
			loadingTimeOutput = CreateDebugLine<Text>(lineParent, settings.diagnosticTextOutputLinePrefab, "Loading times");
			UpdateLoadingTimeOutput();
		#endif
	}

	public static void ShowAd(string AdName)
	{
		bool incentivized = false;
		int index = IndexOfAdPlacement(AdName);
		incentivized = AdIsIncentivized[index];
		PlayNextAvailableAdInList(m_AdSlotFunction, AdName, incentivized);
	}


	// Takes a string and returns the closest available ad network for that string
	private static AdFunction AdSourceFromString(string desiredAdNetwork)
	{
		switch (desiredAdNetwork.ToUpper())
		{

		#if USE_CHARTBOOST
			case "CHARTBOOST":
				return new AdFunction(ChartboostAdNetwork.PlayAd);
		#endif

			case "UNITY":
				return new AdFunction(UnityAdNetwork.PlayAd);

		#if USE_VUNGLE
			case "VUNGLE":
				return new AdFunction(VungleAdNetwork.PlayAd);
		#endif

		#if USE_HEYZAP
			case "HEYZAP":
				return new AdFunction(HeyzapAdNetwork.PlayAd);
		#endif

		#if USE_ADCOLONY
			case "ADCOLONY":
			    return new AdFunction(AdcolonyAdNetwork.PlayAd);
		#endif

			default:
				return new AdFunction(UnityAdNetwork.PlayAd);
		}
	}

	// Goes through the list of ad functions, starting where we left off,
	// and keeps on trying until one plays, or none of them at all would play
	private static void PlayNextAvailableAdInList(AdFunction[] adFunctionList, string placementName, bool incentivized)
	{
		for (int maxTries = adFunctionList.Length; maxTries >= 0; --maxTries)
		{
			int index = StackIndex++;
			bool played = adFunctionList[index](placementName, incentivized);

			#if DEBUG_ADVERTISING

				if (highlightedLine != null)
				{
					highlightedLine.Unhighlight();
				}

				debugLine[index].SetCurrentTime();
				debugLine[index].SetPlacement(string.Format("{0} {1}", incentivized ? "\u2605 " : "", placementName));
				debugLine[index].SetResult(played ? "Played" : "Failed");
				highlightedLine = debugLine[index].HighlightLine();
			#endif

			if (played) return;
		}
	}

	// Takes the placement, and gives you a number (0 - 5) representing the ad
	public static int IndexOfAdPlacement(string placementName)
	{
		int index = Array.IndexOf(AdPlacements, placementName);
		Assert.IsFalse(index == -1, "Unknown ad placement called");
		return index;
	}

#if DEBUG_ADVERTISING
	private static T CreateDebugLine<T>(Transform parent, GameObject prefab, string name)
	{
		GameObject line = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
		line.transform.SetParent(parent);
		line.transform.localScale = Vector3.one;	// inexplicably, the scale was 1.5 or some such
		line.name = name;
		return line.GetComponent<T>();
	}

	public static void UpdateLoadingTimeOutput()
	{
		if (loadingTimeOutput == null)
		{
			return;
		}

		string message;

		if (menuSceneStartTime == 0)
		{
			message = string.Format("Into logo scene at {0:0.00}s\n", logoSceneStartTime);
		}
		else
		{
			message = string.Format("Into menu at {0:0.00}s; logo scene took {1:0.00}s\n", 
				menuSceneStartTime,
				menuSceneStartTime - logoSceneStartTime);
		}

		if (settingsDownloadEndTime != 0)
		{
			message += string.Format("download took {0:0.00}s and {1}\n", 
				settingsDownloadEndTime - settingsDownloadStartTime,
				settingsDownloadedSuccessfully ? " was ok" : "failed");
		}

		message += "Using Unity Ads ";

		#if USE_CHARTBOOST
		message += "+ Chartboost ";
		#endif
		#if USE_VUNGLE
		message += "+ Vungle ";
#endif
        message += string.Format("\nFinal jsonSource = {0}", m_JsonSource);

		loadingTimeOutput.text = message;
	}
#endif

}