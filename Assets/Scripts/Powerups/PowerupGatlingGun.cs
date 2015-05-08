using UnityEngine;
using System.Collections;

public class PowerupGatlingGun : Powerup
{
	public float range = 30.0f;
	public GameObject bulletOriginationPoint;
	public GameObject FX;

	public AudioSource gatlingGunAudio;

	private void Update () 
	{
		if (this.active) 
		{
			if(this.FX != null)
			{
				this.FX.SetActive(true);
			}

			RaycastHit hit;

			Debug.DrawRay (this.bulletOriginationPoint.transform.position, this.transform.forward * this.range, Color.white);

			if (Physics.Raycast (this.bulletOriginationPoint.transform.position, this.transform.forward, out hit, this.range)) {
				if (hit.collider != null && hit.collider.gameObject != null) {
					CartController controller = hit.collider.gameObject.GetComponent<CartController> ();

					if (controller != null) {
						controller.Damage ();
					}
				}
			}

			this.energy -= Time.deltaTime;
		}
		else
		{
			if(this.FX != null)
			{
				this.FX.SetActive(false);
			}
		}

		if(this.energy <= 0.0f)
		{
			Destroy(gameObject);
		}
	}

	public override void Fire (bool on)
	{
		base.Fire (on);

		this.active = on;

		if(this.gatlingGunAudio != null)
		{
			if(this.active)
			{
				this.gatlingGunAudio.Play();
			}
			else
			{
				this.gatlingGunAudio.Stop();
			}
		}
	}
}
