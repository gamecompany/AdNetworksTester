using UnityEngine;
using System.Collections;

public class UnityAdsTester : MonoBehaviour
{
    //
    // UnityAdsTester
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
        UnityAdNetwork.PlayAd(0,()=> { debug.DisplayUnity("Ad played"); }, ()=> { debug.DisplayUnity("Ad is not ready"); });
    }
}
