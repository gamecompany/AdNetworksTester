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
        UIDebugLog.Label label = debug["AdColony"];

        AdcolonyAdNetwork.PlayAd(0, () => label.Update("Ad played"), () => label.Update("Ad is not ready"));
    }
}
