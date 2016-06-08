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

    [SerializeField]
    private UIDebugLog debug;

	// • • • • • • • • • • • • • • • • • • • • //

	//
	// U s e r
	//

    public void PlayAd()
    {
        UIDebugLog.Label label = debug["Chartboost"];

        ChartboostAdNetwork.PlayRewardedAd(() => label.Update("Ad played"), () => label.Update("Ad is not ready"));
    }
}
