using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// This is an object used to display debugging information for advertisements
public class AdDebugLine : MonoBehaviour 
{
	Text[] field;
	Image image;
	Color regularColor;

	// Use this for initialization
	void Awake() 
	{
		field = GetComponentsInChildren<Text>();
		for (int i = 0; i < field.Length; ++i)
		{
			field[i].text = "";
		}
		image = GetComponent<Image>();
		regularColor = image.color;
	}

	public void SetCurrentTime()
	{
		field[0].text = string.Format("@ {0}s", (int)Time.time);
	}

	public void SetProvider(string name)
	{
		field[1].text = name;
	}

	public void SetPlacement(string name)
	{
		field[2].text = name;
	}

	public void SetResult(string result)
	{
		field[3].text = result;
	}

	public void MakeHeader()
	{
		field[0].text = "Time";
		field[1].text = "Ad Network";
		field[2].text = "Placement";
		field[3].text = "Showed ad?";
	}

	public AdDebugLine HighlightLine()
	{
		image.color = new Color(0, 0, 0, 0.5f);
		return this;
	}

	public void Unhighlight()
	{
		image.color = regularColor;
	}
}
