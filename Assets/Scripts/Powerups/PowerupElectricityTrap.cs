using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerupElectricityTrap : Powerup
{
	public GameObject FX;
	public GameObject electricityArc;

	public AudioSource ambientElectricAudio;
	public AudioSource zapAudio;

	public float zapInterval = 0.2f;
	public float arcLifetime = 0.1f;

	public float range = 6.0f;

	private float zapTimer = 0.0f;

	private void Update () 
	{
		if (this.active) 
		{
			if(this.FX != null)
			{
				this.FX.SetActive(true);
			}
			
			this.energy -= Time.deltaTime;

			zapTimer += Time.deltaTime;

			if(zapTimer >= this.zapInterval)
			{
				CartController[] possibleCarts = GameObject.FindObjectsOfType<CartController>();

				List<int> indicies = new List<int>();

				for(int i = 0 ; i < possibleCarts.Length; i++)
				{
					if(Vector3.Distance(possibleCarts[i].transform.position, this.transform.position) <= this.range && (this.parent == null || possibleCarts[i] != this.parent))
					{
						indicies.Add(i);
					}
				}

				if(indicies.Count > 0)
				{
					int chosenCart = indicies[Mathf.FloorToInt(indicies.Count * Random.value)];

					if(chosenCart < possibleCarts.Length && possibleCarts[chosenCart] != null)
					{
						possibleCarts[chosenCart].Damage();
						this.StartCoroutine(this.PlayArc(possibleCarts[chosenCart].transform.position));
						this.zapTimer = 0.0f;
					}
				}
			}
			
			if(this.energy <= 0.0f)
			{
				if(this.parent != null)
				{
					this.parent.activePowerupType = e_PowerupType.NONE;
					this.parent.activePowerup = null;
				}

				this.energy = 5.0f;
				this.gameObject.SetActive(false);
				//Destroy(gameObject);
			}	
		}
		else
		{
			if(this.FX != null)
			{
				this.FX.SetActive(false);
			}
		}
	}
	
	public override void Fire (bool on)
	{
		base.Fire (on);
		
		this.active = on;
		
		if(this.ambientElectricAudio != null)
		{
			if(this.active)
			{
				this.ambientElectricAudio.Play();
			}
			else
			{
				this.ambientElectricAudio.Stop();
			}
		}
	}

	private IEnumerator PlayArc(Vector3 target)
	{
		if(this.electricityArc != null)
		{
			this.electricityArc.transform.LookAt(target, Camera.main.transform.position - this.electricityArc.transform.position);
			this.electricityArc.transform.localScale = new Vector3(3.0f, 3.0f,Vector3.Distance(this.electricityArc.transform.position, target));
			this.electricityArc.SetActive(true);
		}

		yield return new WaitForSeconds (this.arcLifetime);

		if(this.electricityArc != null)
		{
			this.electricityArc.SetActive(false);
		}

	}
}