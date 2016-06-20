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
    private Text console;

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

    public void Display(string text)
    {
        if (!console) return;

        console.text += text + "\n";
    }
}
