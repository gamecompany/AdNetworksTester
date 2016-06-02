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
        // Game Company B (Free game studio)'s Publisher ID is: ef2a0c12b6d378ddce5063ca08a660b9
        HeyzapAds.Start("ef2a0c12b6d378ddce5063ca08a660b9", HeyzapAds.FLAG_NO_OPTIONS);
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
            debug.Display("Ad played");
            // E x i t
            return;
        }
        // Failed debug it
        debug.Display("Ad video not available");
    }


    public void PlayAd()
    {
        // fetch the video
        HZVideoAd.Fetch();
        // Check for availability
        if (HZVideoAd.IsAvailable())
        {
            // Show the ad
            HZVideoAd.Show();
            // Debug it
            debug.Display("Ad played");
            // E x i t
            return;
        }
        // Failed debug it
        debug.Display("Ad video not available");
    }
}
