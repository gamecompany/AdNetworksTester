using UnityEngine;
using System.Collections;

/// <summary>
/// This is a script created to handle the logic for native interface interaction. So that when a user selects 'back' or the 'home' button on their device, the correct logic is performed.
/// </summary>
public class NativeInterfaceHandler : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
    // If the back button is pressed, exit the application
    if(Input.GetKeyDown(KeyCode.Escape)) {
      Debug.Log("BACK BUTTON PRESSED");
      Application.Quit();
    }
	}
}
