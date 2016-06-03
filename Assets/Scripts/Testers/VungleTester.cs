using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VungleTester : MonoBehaviour
{
    [SerializeField]
    private bool selfInitialize;
    public string AndroidID;
    public string iOSID;
    public string WindowsID;

    public UIDebugLog debug;


    // Use this for initialization
    void Start ()
    {
        // E x i t: this will be handle on VungleAdNetwork
        if (!selfInitialize) return;

        Vungle.init(AndroidID, iOSID, WindowsID);

        Vungle.adPlayableEvent += (isAdAvailable) =>
        {
            if (isAdAvailable)
            {
                debug.DisplayVungle("An ad is ready to show!");
            }
            else {
                debug.DisplayVungle("No ad is available at this moment.");
            }
        };
    }

    public void PlayAd()
    {
        VungleAdNetwork.PlayAd(() => { debug.DisplayVungle("Ad played"); }, () => { debug.DisplayVungle("Ad is not avaiable"); });
    }
}
