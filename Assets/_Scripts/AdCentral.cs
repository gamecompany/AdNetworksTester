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

    public enum AdPlacementID
    {
        Ad1Launch,
        Ad2FreeGame,
        Ad3MoreGame,
        Ad4Trailers,
        Ad5Death,
        Ad6PowerUp
    }

    public static bool m_StackCreated = false;
    private static int m_StackIndex = 0;

    public static bool m_SaveLife = false;
    public static string adName;

    public const string AdSettingsFileName = "ten-slot-settings";

    public static string AdSettingsUrl = string.Format("http://topgamestudio.com/DynamicVariables/{0}/ten-slot-settings.json", bundleId);

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

	private static int StackIndex
    {
		get{ return m_StackIndex; }

        set
        {
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
			_jsonText = Resources.Load<TextAsset>(AdSettingsFileName).text;
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
                {
                    jsonText = AdCentralUtility.Fallback;
                }
                break;

			// If we have nothing newer, we use the asset built into the game when it was compiled
			case JsonSource.Asset:
                {
				    var resource = Resources.Load<TextAsset>(AdSettingsFileName);
				    if (resource != null)
				    {
					    jsonText = resource.text;
				    }
                }
                break;

			// If possible, we load the data from a recently-downloaded file
			case JsonSource.File:
                {
                    if (File.Exists(PersistentAdSettingsPath))
				    {
					    jsonText = File.ReadAllText(PersistentAdSettingsPath);
				    }
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

    private static int current_CascadeItem = 0;
    private static int current_TenSlotItem = 0;
    private static int cascadeItems = 0;
    private static int tenSlotItems = 0;

    private static string objectKey = "";
    private static string propertyKey = "";
    private static AdSlotDetails details;
    private static List<string> slotName = new List<string>();
    private static List<AdFunction> slotFunction = new List<AdFunction>();
    private static Dictionary<string, object> jsonDictionary = new Dictionary<string, object>();
    private static Dictionary<string, object> objectDictionay = new Dictionary<string, object>();

	// Takes data in JSON format and parses it. Returns true if it had reasonable data.
	private static AdSlotDetails ParseJsonAdSlots(string jsonText)
	{
        // A result with the details
		details = new AdSlotDetails();
        // We have not validate the result
		details.validResult = false;

		if (string.IsNullOrEmpty(jsonText))
        {
            return details;
		}

        // Clear json dictionay
        if (jsonDictionary.Count > 0) jsonDictionary.Clear();

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

        // Clear the item count (length) of each system
        cascadeItems = tenSlotItems = 0;
        // The dictionary inside our deserialize json (Clear it)
        if(objectDictionay.Count > 0) objectDictionay.Clear();
        // Initialize the lists to store details info (Clear them)
        if (slotName.Count > 0) slotName.Clear();
        if (slotFunction.Count > 0) slotFunction.Clear();
        // the current stack pos and stack order name
        objectKey = "";
        propertyKey = "";

        //
        // C a s c a d e
        //
        GetDetails_ForCascade();
        //
        // 10 - s l o t  m a n a g e r
        //
        GetDaitails_ForTenSlot();

        // Set the current index for each system
        current_CascadeItem = 0;
        current_TenSlotItem = cascadeItems;

        return details;
	}

    private static void GetDetails(string objectName)
    {
        for (int i = 0; i < maxNumberOfSlots; i++)
        {
            objectKey = string.Format("{0}{1}", objectName, i + 1);

            try
            {
                objectDictionay = jsonDictionary[objectKey] as Dictionary<string, object>;
                propertyKey = objectDictionay["Stack Order"] as string;

                if (!string.IsNullOrEmpty(propertyKey))
                {
                    // Name of the ad network
                    slotName.Add(propertyKey);
                    // Get ad function from the ad network name
                    slotFunction.Add(AdSourceFromString(propertyKey));
                    // Add the items for each system
                    if (objectName == "Cascade")
                        cascadeItems++;
                    else
                        tenSlotItems++;
                }
            }
            catch { }
        }

        if (0 == slotName.Count || 0 == slotFunction.Count)
        {
            Debug.LogWarning("We couldn't find any ad-network specified in the advertisment Json source");
        }

        // Set the arrays of details from our list
        try
        {
            // Try to parse the lists
            details.slotName = slotName.ToArray();
            details.slotFunction = slotFunction.ToArray();
            // Set the valid result from the length of the arrays
            details.validResult =
                details.slotName.Length == details.slotFunction.Length &&
                details.slotName.Length > 0;
        }
        catch { }
    }

    private static void GetDetails_ForCascade()
    {
        Debug.LogFormat("GetDetails_ForCascade");
        GetDetails("Cascade");
    }

    private static void GetDaitails_ForTenSlot()
    {
        GetDetails("StackPos");
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

	public static void ShowAd(string adPlacement)
	{
		bool incentivized = false;
		int index = IndexOfAdPlacement(adPlacement);
		incentivized = AdIsIncentivized[index];
        
        if(current_CascadeItem < cascadeItems)
        {
            // Reset the index of the 10 slot system if we haven't or we already went throught all of it
            current_TenSlotItem = cascadeItems;
            // Cascade
            if(PlayCascadeAvaiableAdList(m_AdSlotFunction, adPlacement, incentivized))
            {
                // E x i t
                return;
            }
        }
        // Ten slot system
		PlayNextAvailableAdInList(m_AdSlotFunction, adPlacement, incentivized);
        // Reset the index of the cascade system if we went through all the ten slot system
        if (current_TenSlotItem >= (tenSlotItems + cascadeItems)) current_CascadeItem = 0;
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

        #if USE_UNITYADS
            case "UNITY":
				return new AdFunction(UnityAdNetwork.PlayAd);
        #endif

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
        #if USE_UNITYADS
				return new AdFunction(UnityAdNetwork.PlayAd);
        #else
                return null;
        #endif
        }
	}

    public static void IncrementCurrentCascadeIndex()
    {
        current_CascadeItem = Mathf.Min(cascadeItems, current_CascadeItem + 1);
    }

    private static bool PlayCascadeAvaiableAdList(AdFunction[] adFunctionList, string placementName, bool incentivized)
    {
        bool played = false;

        for(int i = current_CascadeItem; current_CascadeItem < cascadeItems; ++i)
        {
            played = adFunctionList[current_CascadeItem](placementName, incentivized);

#if DEBUG_ADVERTISING

            if (highlightedLine != null)
            {
                highlightedLine.Unhighlight();
            }

            debugLine[current_CascadeItem].SetCurrentTime();
            debugLine[current_CascadeItem].SetPlacement(string.Format("{0} {1}", incentivized ? "\u2605 " : "", placementName));
            debugLine[current_CascadeItem].SetResult(played ? "Cascade ad played" : "Cascade ad not avaible");
            highlightedLine = debugLine[current_CascadeItem].HighlightLine();
#endif

            if (played)
            {
                return true;
            }
            else
            {
                current_CascadeItem++;
            }

            Debug.LogFormat("Play cascade-> Index: {0}", current_CascadeItem);
        }

        return false;
    }

	// Goes through the list of ad functions, starting where we left off,
	// and keeps on trying until one plays, or none of them at all would play
	private static void PlayNextAvailableAdInList(AdFunction[] adFunctionList, string placementName, bool incentivized)
	{
		for (int i = current_TenSlotItem; i < (tenSlotItems + cascadeItems); ++i)
		{
            bool played = adFunctionList[i](placementName, incentivized);

#if DEBUG_ADVERTISING

				if (highlightedLine != null)
				{
					highlightedLine.Unhighlight();
				}

				debugLine[i].SetCurrentTime();
				debugLine[i].SetPlacement(string.Format("{0} {1}", incentivized ? "\u2605 " : "", placementName));
				debugLine[i].SetResult(played ? "Played" : "Not avaible");
				highlightedLine = debugLine[i].HighlightLine();
#endif
            
            Debug.LogFormat("Play ten slot-> Index: {0}", current_TenSlotItem);

            current_TenSlotItem++;

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

#if USE_UNITYADS
        message += "Using Unity Ads ";
#endif

#if USE_UNITYADS
        message += "+ Heyzap";
#endif

#if USE_CHARTBOOST
        message += "+ Chartboost ";
#endif

#if USE_VUNGLE
		message += "+ Vungle ";
#endif
        
#if USE_UNITYADS
        message += "+ Adcolony";
#endif
        message += string.Format("\nFinal jsonSource = {0}", m_JsonSource);

		loadingTimeOutput.text = message;
	}
#endif

    public static string RandomAdPlacement()
    {
        int randomID = UnityEngine.Random.Range(0, 2);

        string id = "";

        switch (randomID)
        {
            case 0: id = AdPlacementID.Ad3MoreGame.ToString(); break;
            case 1: id = AdPlacementID.Ad4Trailers.ToString(); break;
        }

        return id;
    }

    public static string RandomRewardedAdPlacement()
    {
        int randomID = UnityEngine.Random.Range(0, 2);

        string id = "";

        switch (randomID)
        {
            case 0: id = AdPlacementID.Ad1Launch.ToString(); break;
            case 1: id = AdPlacementID.Ad5Death.ToString(); break;
        }

        return id;
    }

}

public struct AdCentralUtility
{
    public static string Fallback =
    @"{
        ""Cascade1"":{""Stack Order"":""Unity""},
        ""Cascade2"":{""Stack Order"":""Unity""},
        ""Cascade3"":{""Stack Order"":""Unity""},

        ""StackPos1"":{""Stack Order"":""Unity""},
        ""StackPos2"":{""Stack Order"":""Unity""},
        ""StackPos3"":{""Stack Order"":""Unity""},
        ""StackPos4"":{""Stack Order"":""Unity""},
        ""StackPos5"":{""Stack Order"":""Unity""}
    }";
}