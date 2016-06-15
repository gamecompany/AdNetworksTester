using UnityEngine;
using System.Collections;

public class mNectarTester : MonoBehaviour
{
    //
    // mNectarTester
    // structs & classes

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // V a r i a b l e s
    //

    [SerializeField]
    private UIDebugLog debug;
    [SerializeField]
    private string rewardedAdUnitID;
    [SerializeField]
    private string interstitialAdUnitID;

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
        // Initialize
        MNectar.initAdUnit(rewardedAdUnitID);
        MNectar.initInterstitialAdUnit(interstitialAdUnitID);
        // Request
        MNectar.requestRewardable(rewardedAdUnitID);
        MNectar.requestInterstitial(interstitialAdUnitID);
    }

	// • • • • • • • • • • • • • • • • • • • • //

	//
	// U s e r
	//

    public void PlayRewardableAd()
    {
        if(MNectar.isRewardableReady(rewardedAdUnitID))
        {
            MNectar.showRewardable(rewardedAdUnitID);
            MNectar.requestRewardable(rewardedAdUnitID);

            debug.Display("Rewarded ad played");

            return;
        }

        debug.DisplayError("Rewarded not ready");
    }
    public void PlayInterstitialAd()
    {
        if (MNectar.isInterstitialReady(interstitialAdUnitID))
        {
            MNectar.showInterstitial(interstitialAdUnitID);
            MNectar.requestInterstitial(interstitialAdUnitID);

            debug.Display("Interstitial ad played");

            return;
        }

        debug.DisplayError("Interstitial not ready");
    }
}
