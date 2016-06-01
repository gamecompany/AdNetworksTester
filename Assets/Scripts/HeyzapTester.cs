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
        HeyzapAds.Start("cde2146866f96c9ad90736c1af512e13", HeyzapAds.FLAG_NO_OPTIONS);
    }

	// • • • • • • • • • • • • • • • • • • • • //

	//
	// U s e r
	//

    public void PlayAd()
    {
        HZVideoAd.Fetch();

        if(!HZVideoAd.IsAvailable())
        {
            debug.Display("Ad video not available");

            return;
        }

        HZVideoAd.Show();

        //HZInterstitialAd.Show();

        debug.Display("Ad played");
    }
}
