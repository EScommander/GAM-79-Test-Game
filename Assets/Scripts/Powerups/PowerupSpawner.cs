using UnityEngine;
using System.Collections;

public class PowerupSpawner : MonoBehaviour 
{
	public Powerup[] powerupPrefabs;
	public GameObject[] displayObjects;

	public Vector2 minMaxChangeTime;

	private float changeTime = 0.0f;
	private float timer = 0.0f;
	private int chosenPowerup;

	private void Start()
	{
		this.changeTime = Random.Range (minMaxChangeTime.x, minMaxChangeTime.y);
		this.chosenPowerup = 0;
	}

	private void Update()
	{
		this.timer += Time.deltaTime;

		if(this.timer >= this.changeTime)
		{
			this.timer = 0.0f;
			this.changeTime = Random.Range (minMaxChangeTime.x, minMaxChangeTime.y);
			this.chosenPowerup = Mathf.FloorToInt(Random.value * powerupPrefabs.Length);

			for(int i = 0 ; i < this.powerupPrefabs.Length; i++)
			{
				if(this.displayObjects[i] != null)
				{
					if(i == this.chosenPowerup)
					{
						this.displayObjects[i].SetActive(true);
					}
					else
					{
						this.displayObjects[i].SetActive(false);
					}
				}
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other != null && other.gameObject != null)
		{
			CartController cart = other.gameObject.GetComponent<CartController>();

			if(cart != null)
			{
				cart.activePowerupType = powerupPrefabs[this.chosenPowerup].powerupType;
			}
		}
	}
}
