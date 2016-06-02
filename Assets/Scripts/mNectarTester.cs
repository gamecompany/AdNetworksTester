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
    private int adSelected;
    [SerializeField]
    private string[] AD_UNIT_ID;

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // P r o p e r t i e s
    //

    public bool validAdID
    {
        get { return -1 != adSelected; }
    }

    public string AdSelected
    {
        get { return AD_UNIT_ID[adSelected]; }
    }

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // U n i t y
    //

    void OnValidate()
    {
        if (null == AD_UNIT_ID || 0 == AD_UNIT_ID.Length)
            adSelected = -1;
    }

    void Start()
    {
        if(validAdID)
        {
            MNectar.initAdUnit(AdSelected);
        }
    }

	// • • • • • • • • • • • • • • • • • • • • //

	//
	// U s e r
	//

    public void PlayRewardableAd()
    {
        // E x i t
        if (!validAdID) return;

        MNectar.requestRewardable(AdSelected);

        if(MNectar.isRewardableReady(AdSelected))
        {
            MNectar.showRewardable(AdSelected);

            debug.Display("Ad played");

            return;
        }

        debug.Display("Ad failed");
    }
}
