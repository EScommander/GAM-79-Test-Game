using UnityEngine;
using System.Collections;

public class StartScreenInput : MonoBehaviour 
{
	//public Object jukebox = null;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.anyKey)
		{
			//DontDestroyOnLoad(jukebox);
			Application.LoadLevel("Menu_CharacterSelect");
		}
	}
}
