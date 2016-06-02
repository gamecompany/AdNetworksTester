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

    private Color consoleNormalTextColor;

    [SerializeField]
    private Text console;

	// • • • • • • • • • • • • • • • • • • • • //

	//
	// P r o p e r t i e s
	//

    public Color NormalColorText
    {
        get { return consoleNormalTextColor; }
    }
	
	// • • • • • • • • • • • • • • • • • • • • //

	//
	// U n i t y
	//

    void Start()
    {
        consoleNormalTextColor = console.color;
    }

	// • • • • • • • • • • • • • • • • • • • • //

	//
	// U s e r
	//

    public void Display(string text)
    {
        if (!console) return;

        console.color = NormalColorText;
        console.text = text;
    }
    public void DisplayError(string text)
    {
        if (!console) return;

        console.color = Color.red;
        console.text = text;
    }
}
