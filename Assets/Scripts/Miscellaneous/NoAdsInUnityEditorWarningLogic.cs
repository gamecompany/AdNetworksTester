using UnityEngine;
using System.Collections;

public class NoAdsInUnityEditorWarningLogic : MonoBehaviour {

  public bool displayWarningText = false;

  void Awake() {
    #if UNITY_EDITOR
      displayWarningText = true;
    #endif
  }

	// Use this for initialization
	void Start () {
    if(displayWarningText) {
      this.gameObject.GetComponent<GUIText>().color = Color.red;
    }
	}

	// Update is called once per frame
	void Update () {

	}
}
