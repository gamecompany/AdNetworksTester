using UnityEngine;

public class ChartboostTester : MonoBehaviour
{
    //
    // ChartboostTester
    // structs & classes

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // V a r i a b l e s
    //

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // U s e r
    //

    public void PlayAd()
    {
#if USE_CHARTBOOST
        // E x i t
        if (!AdTester.Exists) return;
        // Find the label
        AdTester.Label label = AdTester.I["Chartboost"];
        // E x i t
        if (null == label) return;
        // Random ad placement id
        string id = AdTester.RandomAdPlacement();
        // Update the label
        label.Update(ChartboostAdNetwork.PlayAd(id, false) ? "(" + id + ") Rewarded ad played" : "(" + id + ")Rewarded ad is not ready");
#endif
    }

    public void PlayRewardedAd()
    {
#if USE_CHARTBOOST
        // E x i t
        if (!AdTester.Exists) return;
        // Find the label
        AdTester.Label label = AdTester.I["Chartboost"];
        // E x i t
        if (null == label) return;
        // Random ad placement id
        string id = AdTester.RandomRewardedAdPlacement();
        // Update the label
        label.Update(ChartboostAdNetwork.PlayAd(id, true) ? "(" + id + ") Rewarded ad played" : "(" + id + ")Rewarded ad is not ready");
#endif
    }
}
