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

        UIDebugLog.Label label = debug["Vungle"];

        Vungle.adPlayableEvent += (isAdAvailable) =>
        {
            if (isAdAvailable)
            {
                label.Update("An ad is ready to show!");
            }
            else {
                label.Update("No ad is available at this moment.");
            }
        };
    }

    public void PlayAd()
    {
        UIDebugLog.Label label = debug["Vungle"];

        VungleAdNetwork.PlayAd(() => { label.Update("Ad played"); }, () => { label.Update("Ad is not avaiable"); });
    }
}
