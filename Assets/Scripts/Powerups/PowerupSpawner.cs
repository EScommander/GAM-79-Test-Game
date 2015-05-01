using UnityEngine;
using System.Collections;

public class PowerupSpawner : MonoBehaviour 
{
	public Powerup powerupPrefab;

	private void OnTriggerEnter(Collider other)
	{
		if(other != null && other.gameObject != null)
		{
			CartController cart = other.gameObject.GetComponent<CartController>();

			if(cart != null)
			{
				cart.activePowerup = powerupPrefab;
				cart.PowerupInit();
			}
		}
	}
}
