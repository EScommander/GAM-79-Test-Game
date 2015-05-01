using UnityEngine;
using System.Collections;

public class PowerupGatlingGun : Powerup
{
	public float range = 30.0f;
	public GameObject bulletOriginationPoint;
	public ParticleSystem gatlingGun;

	private void Update () 
	{
		RaycastHit hit;

		Debug.DrawRay (this.bulletOriginationPoint.transform.position, this.transform.forward * this.range, Color.white);

		if(Physics.Raycast(this.bulletOriginationPoint.transform.position, this.transform.forward, out hit, this.range))
		{
			if(hit.collider != null && hit.collider.gameObject != null)
			{
				CartController controller = hit.collider.gameObject.GetComponent<CartController>();

				if(controller != null)
				{
					controller.Damage();
				}
			}
		}

		this.energy -= Time.deltaTime;

		if(this.energy <= 0.0f)
		{
			Destroy(gameObject);
		}
	}
}
