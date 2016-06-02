using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MNectarDemoScript : MonoBehaviour {

	private string adUnitId;

	[SerializeField] private InputField AdUnitInputField;

	[SerializeField] private Text AdUnitInfo;

	[SerializeField] private Text ReadinessInfo;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setAdUnit()
	{
		adUnitId = AdUnitInputField.text;
		Debug.Log("Ad Unit: "+AdUnitInputField.text);
		AdUnitInfo.text = "Ad Unit: " + adUnitId;

		//Custom Parameters are used for server postbacks when rewarded, they are not necessary

		Dictionary<string, string> parameters = new Dictionary<string, string>();
		parameters.Add ("pub_param1", "value 1");
		parameters.Add ("pub_param2", "value 2");
		parameters.Add ("pub_param3", "value 3");

		MNectar.initAdUnit (adUnitId);

		//If using custom parameters and server postbacks then use this function to initialize your ad unit instead

		//MNectar.initAdUnit (adUnitId, parameters);
	}

	public void loadAd()
	{
		MNectar.requestRewardable (adUnitId);
	}

	public void showAd()
	{
		MNectar.showRewardable (adUnitId);
		ReadinessInfo.text = "Ad not ready";
	}
}
