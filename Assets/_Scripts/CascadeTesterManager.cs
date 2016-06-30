using UnityEngine;
using System.Collections;

public class CascadeTesterManager : MonoBehaviour
{
	//
	// CascadeTesterManager
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
        AdCentral.ShowAd(AdCentral.RandomNormalAdPlacement());
    }

    public void PlayRewardedAd()
    {
        AdCentral.ShowAd(AdCentral.RandomRewardedAdPlacement());
    }

    public void PlayTenSlotManager()
    {
        AdCentral.ShowAdFromTenSlotSystem(AdCentral.RandomAdPlacement());
    }
}
