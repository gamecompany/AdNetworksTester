using UnityEngine;
using Heyzap;

public class HeyzapTester : MonoBehaviour
{
    //
    // HeyzapTester
    // structs & classes

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // V a r i a b l e s
    //

    [SerializeField]
    private bool selfInitialize;
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
        // E x i t: this will be handle on HeyzapAdNetwork
        if (!selfInitialize) return;

        HeyzapAds.Start("cde2146866f96c9ad90736c1af512e13", HeyzapAds.FLAG_NO_OPTIONS);
    }

	// • • • • • • • • • • • • • • • • • • • • //

	//
	// U s e r
	//
    
    public void PlayAd()
    {
        UIDebugLog.Label label = debug["Heyzap"];

        HeyzapAdNetwork.PlayAd(() => label.Update("Ad played"), () => label.Update("Ad video not available"));
    }
}
