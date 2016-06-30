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

    [SerializeField]
    private GameObject[] leftColumn;

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

    public void PlayTenSlotAd()
    {
        AdCentral.ShowAdFromTenSlotSystem(AdCentral.RandomNormalAdPlacement());
    }

    public void PlayRewardedTenSlotAd()
    {
        AdCentral.ShowAdFromTenSlotSystem(AdCentral.RandomRewardedAdPlacement());
    }

    public void ActiveLeftColumn(bool value)
    {
        for (int i = 0; i < leftColumn.Length; i++)
            leftColumn[i].SetActive(value);
    }
}
