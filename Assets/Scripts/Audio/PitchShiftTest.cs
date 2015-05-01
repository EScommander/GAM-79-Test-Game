using UnityEngine;
using System.Collections;

public class PitchShiftTest : MonoBehaviour 
{
	public AudioSource clip;
	public float pitch = 1;
	public float stepUp = 0.1f;

	public float maxPitch = 2.0f;
	public float minPitch = 0.0f;

	private void Start()
	{
		this.pitch = this.clip.pitch;
	}

	public void Update () 
	{
		this.clip.pitch = this.pitch;

		this.pitch += Time.deltaTime * this.stepUp;

		if(this.pitch >= this.maxPitch)
		{
			this.pitch = this.maxPitch;
		}
		else if(this.pitch <= this.minPitch)
		{
			this.pitch = this.minPitch;
		}
	}
}
