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
        MNectar.initAdUnit(rewardedAdUnitID);
        MNectar.initAdUnit(interstitialAdUnitID);
    }

	// • • • • • • • • • • • • • • • • • • • • //

	//
	// U s e r
	//

    public void PlayRewardableAd()
    {
        MNectar.requestRewardable(rewardedAdUnitID);

        if(MNectar.isRewardableReady(rewardedAdUnitID))
        {
            MNectar.showRewardable(rewardedAdUnitID);

            debug.Display("Rewarded ad played");

            return;
        }

        debug.DisplayError("Rewarded not ready");
    }
    public void PlayInterstitialAd()
    {
        MNectar.requestInterstitial(interstitialAdUnitID);

        if (MNectar.isInterstitialReady(interstitialAdUnitID))
        {
            MNectar.showInterstitial(interstitialAdUnitID);

            debug.Display("Interstitial ad played");

            return;
        }

        debug.DisplayError("Interstitial not ready");
    }
}
