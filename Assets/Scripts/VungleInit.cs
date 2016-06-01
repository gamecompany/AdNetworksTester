using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VungleInit : MonoBehaviour
{
    public string AndroidID;
    public string iOSID;
    public string WindowsID;

    public UIDebugLog DebugLog;


    // Use this for initialization
    void Start ()
    {
        Vungle.init(AndroidID, iOSID, WindowsID);

        Vungle.adPlayableEvent += (isAdAvailable) => {
            if (isAdAvailable)
            {
                DebugLog.DisplayVungle("An ad is ready to show!");
            }
            else {
                DebugLog.DisplayVungle("No ad is available at this moment.");
            }
        };
    }

    public void WatchAd()
    {
        Dictionary<string, object> options = new Dictionary<string, object>();
        options["incentivized"] = true;
        Vungle.playAdWithOptions(options);
    }
}
