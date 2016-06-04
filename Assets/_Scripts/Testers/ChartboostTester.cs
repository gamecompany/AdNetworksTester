using UnityEngine;
#if USE_CHARTBOOST
using ChartboostSDK;
#endif

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
        UIDebugLog.Label label = debug["Chartboost"];

        ChartbootAdNetwork.PlayAd(() => label.Update("Ad played"), () => label.Update("Ad is not ready"));
    }
}
