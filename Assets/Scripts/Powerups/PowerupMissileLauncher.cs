using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerupMissileLauncher : Powerup 
{
	public GameObject targetingFXPrefab = null;
	public GameObject targetingFXClone = null;

	public GameObject targetedCart = null;

	public int selectedTarget = 0;

	private float targetingUpdateDelay = 0.2f;
	private float targetingUpdateTimer = 0.0f;

	private void Update()
	{

	}

	private void UpdateTarget()
	{
		for(int i = 0 ; i < GameManager.SceneInstance.activeCarts.Count; i++)
		{

		}
	}

	public void IncrementTarget()
	{
		if(GameManager.SceneInstance.activeCarts.Count > this.selectedTarget)
		{
			this.selectedTarget = 0;
		}
		else if(GameManager.SceneInstance.activeCarts.Count <= 0)
		{
			this.selectedTarget = -1;
		}
	}

	public override void Use ()
	{
		this.energy -= 1.0f;
	}
}
