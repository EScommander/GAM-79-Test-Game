using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour 
{
	public string name = "Powerup";

	public void OnTriggerEnter(Collider collider)
	{
		CartController cart = collider.gameObject.GetComponent<CartController> ();

		if(cart != null)
		{

		}
	}
}
