using UnityEngine;
using System.Collections;

public class CollisionAudioFX : MonoBehaviour 
{
	public AudioSource[] genericCollisionAudio;
	public AudioSource[] cartCollisionAudio;

	public AudioSource[] genericCollisionVoice;

	public GameObject[] FXObjs;

	public bool isColliding = false;

	public float cooldown = 2.0f;

	private float cooldownTimer = 0.0f;

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

		if(this.cooldownTimer > 0)
		{
			return;
		}

		this.cooldownTimer = this.cooldown;

		foreach(AudioSource source in genericCollisionAudio)
		{
			if(source != null)
			{
				source.Play();
			}
		}

		if(this.genericCollisionVoice != null && this.genericCollisionVoice.Length > 0)
		{
			int chosenVoice = Mathf.FloorToInt(Random.value * this.genericCollisionVoice.Length);

			if(chosenVoice < this.genericCollisionVoice.Length && this.genericCollisionVoice[chosenVoice] != null)
			{
				this.genericCollisionVoice[chosenVoice].Play();
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
				fxObj.SetActive(true);
			}
		}

		this.isColliding = true;
	}

	private void Update()
	{
		if(this.cooldownTimer > 0.0f)
		{
			this.cooldownTimer -= Time.deltaTime;
		}
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
