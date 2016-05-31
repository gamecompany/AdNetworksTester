using UnityEngine;

public class CounterText : MonoBehaviour {
  public GUIText counterText = null;
  public string counterName = null;

  void Start() {
    counterText.text = getText();
  }

  void Update() {
    counterText.text = getText();
  }

  private string getText() {
    return counterName + " callback called " + ADCAdManager.Instance.GetCounter(counterName).ToString() + " time(s)";
  }
}
