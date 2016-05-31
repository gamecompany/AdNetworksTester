using UnityEngine;
using System.Collections;

/// <summary>
/// This was created to update the currency text on screen by querying the ADCAdManager
/// </summary>
public class CurrencyTextManager : MonoBehaviour {

  public GUIText currencyText = null;
  public GameObject currencyButton = null;

	// Use this for initialization
	void Start () {
    PlayVideoZoneButton playVideoZoneButton = currencyButton.GetComponent<PlayVideoZoneButton>();
    currencyText.text = (AdColony.GetV4VCName(ADCAdManager.GetZoneIdByKey(playVideoZoneButton.zoneIdKey)) + ": " + ADCAdManager.GetRegularCurrencyAmount());
	}

	// Update is called once per frame
	void Update () {
    // currencyText.text = ("Credits: " + ADCAdManager.GetRegularCurrencyAmount());
    PlayVideoZoneButton playVideoZoneButton = currencyButton.GetComponent<PlayVideoZoneButton>();
    currencyText.text = (AdColony.GetV4VCName(ADCAdManager.GetZoneIdByKey(playVideoZoneButton.zoneIdKey)) + ": " + ADCAdManager.GetRegularCurrencyAmount());
	}
}
