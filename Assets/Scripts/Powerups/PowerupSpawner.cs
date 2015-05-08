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
				if(cart.activePowerup != null)
				{
					Destroy (cart.activePowerup.gameObject);
				}

				cart.activePowerup = powerupPrefab;
				cart.PowerupInit();
			}
		}
	}
}
