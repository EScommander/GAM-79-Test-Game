using UnityEngine;
using System.Collections;

public class CollisionAudioFX : MonoBehaviour 
{
	public AudioSource[] genericCollisionAudio;
	public AudioSource[] cartCollisionAudio;

	public GameObject[] FXObjs;

	public bool isColliding = false;

	private void Awake()
	{
		BoxCollider collider = this.gameObject.GetComponent<BoxCollider> ();

		if(collider != null)
		{
			collider.isTrigger = true;
		}

		foreach(AudioSource source in genericCollisionAudio)
		{
			if(source != null)
			{
				source.playOnAwake = false;
			}
		}

		foreach(AudioSource source in cartCollisionAudio)
		{
			if(source != null)
			{
				source.playOnAwake = false;
			}
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		if(collision.tag == "Powerup")
		{
			return;
		}

		foreach(AudioSource source in genericCollisionAudio)
		{
			if(source != null)
			{
				source.Play();
			}
		}

		if(collision.gameObject != null && collision.gameObject.GetComponent<CartController>() != null)
		{
			foreach(AudioSource source in cartCollisionAudio)
			{
				if(source != null)
				{
					source.Play();
				}
			}
		}

		foreach(GameObject fxObj in FXObjs)
		{
			if(fxObj != null)
			{
				fxObj.SetActive(false);
			}
		}

		this.isColliding = true;
	}

	private void OnTriggerExit(Collider collision)
	{
		/*
		foreach(AudioSource source in genericCollisionAudio)
		{
			if(source != null)
			{
				source.Stop();
			}
		}

		if(collision.gameObject != null && collision.gameObject.GetComponent<CartController>() != null)
		{
			foreach(AudioSource source in cartCollisionAudio)
			{
				if(source != null)
				{
					source.Stop();
				}
			}
		}*/

		foreach(GameObject fxObj in FXObjs)
		{
			if(fxObj != null)
			{
				fxObj.SetActive(false);
			}
		}

		this.isColliding = false;
	}
}
