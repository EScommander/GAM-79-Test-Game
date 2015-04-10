using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestUI : MonoBehaviour 
{
	Text characterName;
	Image characterImage;
	// Use this for initialization
	void Start () 
	{
		characterName = transform.GetChild(0).GetComponent<Text> ();
		characterName.text = "Bloop";
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void SetImage()
	{

	}
}
