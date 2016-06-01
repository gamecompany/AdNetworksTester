using UnityEngine;
using UnityEngine.UI;

public class UIDebugLog : MonoBehaviour
{
    //
    // UIDebugLog
    // structs & classes

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // V a r i a b l e s
    //

    [SerializeField]
    private Text heyzap;
    [SerializeField]
    private Text chartboost;
    [SerializeField]
    private Text vungle;

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

    public void DisplayHeyzap(string text)
    {
        if (!heyzap) return;

        heyzap.text = "Heyzap:" + text;
    }

    public void DisplayChartboost(string text)
    {
        if (!chartboost) return;

        chartboost.text = "Chartboost" + text;
    }

    public void DisplayVungle(string text)
    {
        if (!vungle) return;

        vungle.text = "Chartboost" + text;
    }
}
