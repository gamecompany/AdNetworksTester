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

    public void PlayAdWithTag(string hzShowOptionTag)
    {
        // default tag
        string optionTag = hzShowOptionTag;
        // fetch the video
        HZVideoAd.Fetch(optionTag);
        // Create show options
        HZShowOptions showOptions = new HZShowOptions();
        showOptions.Tag = optionTag;
        // Check for availability
        if (HZVideoAd.IsAvailable(optionTag))
        {
            // Show the ad
            HZVideoAd.ShowWithOptions(showOptions);
            // Debug it
            debug.DisplayHeyzap("Ad played");
            // E x i t
            return;
        }
        // Failed debug it
        debug.DisplayHeyzap("Ad video not available");
    }


    public void PlayAd()
    {
        HeyzapAdNetwork.PlayAd(() => debug.DisplayHeyzap("Ad played"), () => debug.DisplayHeyzap("Ad video not available"));
    }
}
