using UnityEngine;

public class AdColonyTester : MonoBehaviour
{
    //
    // AdColony
    // structs & classes

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // V a r i a b l e s
    //

    [SerializeField]
    private string androidAppVersion = "version:1.0,store:google";
    [SerializeField]
    private string appID = "app4a74095801a64b1098";
    [SerializeField]
    private string[] zones = new string[] { "vz314caedd4cfd445297" };
    [SerializeField]
    private UIDebugLog debug;

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // P r o p e r t i e s
    //

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // U n i t y
    //

    void Start()
    {
        AdColony.Configure(androidAppVersion, appID, zones);
    }

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // U s e r
    //

    public void PlayMoreGames()
    {
        bool result = AdColony.ShowVideoAd(zones[0]) && AdColony.ShowVideoAd(zones[0]);

        debug.DisplayAdColony(result ? "Play ad" : "Failed");
    }
}
