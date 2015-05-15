using UnityEngine;
using System.Collections;

public class RaceAudio : MonoBehaviour 
{
	public AudioSource finalLapSource = null;

	public bool finalLap = false;
	// Use this for initialization
	void Awake () 
	{
		finalLapSource.volume = 0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(finalLap)
		{
			finalLapSource.volume = 1f;
		}
	}
}
