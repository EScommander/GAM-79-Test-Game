using UnityEngine;
using System.Collections;

public class DeadZone : MonoBehaviour 
{
	public float respawnDelay = 2.0f;

	private void OnTriggerEnter(Collider other)
	{
		CartController cart = other.gameObject.GetComponent<CartController> ();

		if(cart != null && !cart.respawning)
		{
			this.StartCoroutine(cart.ResetCart(respawnDelay));
		}
	}

	private void OnTriggerStay(Collider other)
	{
		CartController cart = other.gameObject.GetComponent<CartController> ();
		
		if(cart != null && !cart.respawning)
		{
			this.StartCoroutine(cart.ResetCart(respawnDelay));
		}
	}
}
