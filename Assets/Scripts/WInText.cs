using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WInText : MonoBehaviour 
{
	public static WInText instance;
	public Text text;

	private void Start()
	{
		WInText.instance = this;
		text.text = "";
	}
}
