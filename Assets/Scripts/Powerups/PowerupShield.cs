using UnityEngine;
using System.Collections;

public class PowerupShield : Powerup
{
	public GameObject FX;
	
	public AudioSource shieldAudio;
	
	private void Update () 
	{
		if (this.active) 
		{
			if(this.FX != null)
			{
				this.FX.SetActive(true);
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
			if(this.parent != null)
			{
				this.parent.activePowerupType = e_PowerupType.NONE;
				this.parent.activePowerup = null;
			}
			this.gameObject.SetActive(false);
		}
	}
	
	public override void Fire (bool on)
	{
		base.Fire (on);
		
		this.active = on;
		
		if(this.shieldAudio != null)
		{
			if(this.active)
			{
				this.shieldAudio.Play();
			}
			else
			{
				this.shieldAudio.Stop();
			}
		}
	}
}