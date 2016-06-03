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
    [SerializeField]
    private Text Adcolony;
    [SerializeField]
    private Text unity;

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

        heyzap.text = "Heyzap: " + text;
    }

    public void DisplayChartboost(string text)
    {
        if (!chartboost) return;

        chartboost.text = "Chartboost: " + text;
    }

    public void DisplayVungle(string text)
    {
        if (!vungle) return;

        vungle.text = "Vungle: " + text;
    }

    public void DisplayAdColony(string text)
    {
        if (!Adcolony) return;

        Adcolony.text = "Adcolony: " + text;
    }

    public void DisplayUnity(string text)
    {
        if (!unity) return;

        unity.text = "Unity: " + text;
    }
}
