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
    
    // • • • • • • • • • • • • • • • • • • • • //

    //
    // P r o p e r t i e s
    //

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // U n i t y
    //
    
    // • • • • • • • • • • • • • • • • • • • • //

    //
    // U s e r
    //

    public void PlayAd()
    {
#if USE_ADCOLONY
        // E x i t
        if (!AdTester.Exists) return;
        // Find the label
        AdTester.Label label = AdTester.I["AdColony"];
        // E x i t
        if (null == label) return;
        // Random ad placement id
        string id = AdTester.RandomAdPlacement();
        // Update the label
        label.Update(AdcolonyAdNetwork.PlayAd(id, false) ? "(" + id + ") Rewarded ad played" : "(" + id + ")Rewarded ad is not ready");
#endif
    }

    public void PlayRewardedAd()
    {
#if USE_ADCOLONY
        // E x i t
        if (!AdTester.Exists) return;
        // Find the label
        AdTester.Label label = AdTester.I["AdColony"];
        // E x i t
        if (null == label) return;
        // Random ad placement id
        string id = AdTester.RandomRewardedAdPlacement();
        // Update the label
        label.Update(AdcolonyAdNetwork.PlayAd(id, true) ? "("+id+") Rewarded ad played" : "("+id+")Rewarded ad is not ready");
#endif
    }
}
