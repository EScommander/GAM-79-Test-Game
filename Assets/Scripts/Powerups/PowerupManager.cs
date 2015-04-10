using UnityEngine;
using System.Collections;

public class PowerupManager : MonoBehaviour 
{
	public static PowerupManager SceneInstance;

	public Powerup[] powerups;

	public float[] totalChances;

	private void Start()
	{
		PowerupManager.SceneInstance = this;

		if(this.powerups.Length > 0)
		{
			totalChances = new float[this.powerups[0].chancesPerPlace.Length];
			for(int i = 0 ; i < this.powerups[0].chancesPerPlace.Length; i++)
			{
				float totalChance = 0.0f;

				for(int a = 0 ; a < this.powerups.Length; a++)
				{
					totalChance += this.powerups[a].chancesPerPlace[i];
				}
				totalChances[i] = totalChance;
			}
		}
	}

	public int PickupPowerup(int place)
	{
		if(place == -1)
		{
			return Mathf.FloorToInt(Random.value * powerups.Length);
		}

		float randomValue = Random.Range (0.0f, this.totalChances [place]);

		float[] neededValues = new float[this.powerups.Length];

		neededValues [0] = this.powerups [0].chancesPerPlace[place];

		int chosenPowerup = 0;

		for(int i = 1 ; i < this.powerups.Length; i++)
		{
			neededValues[i] = this.powerups [i-1].chancesPerPlace[place] + this.powerups [i].chancesPerPlace[place];

			if(neededValues[i] < randomValue)
			{
				return i; //TODO
			}
		}

		return Mathf.FloorToInt(Random.value * powerups.Length); //TODO
	}
}
