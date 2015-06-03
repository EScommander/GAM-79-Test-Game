using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour 
{
	public enum e_PowerupType{NONE, GATLINGGUN, ELECTRICTRAP,SHIELD};
	public e_PowerupType powerupType;

	public string Name = "Powerup";
	public float[] chancesPerPlace;
	public GameObject prefab;

	public enum e_connectionTypes{LEFT, RIGHT, TOP, BACK};
	public e_connectionTypes connectionType;

	//this holds all damage, shields, power-esque values
	public float power = 1.0f;

	//this holds all health, time, ammo, energy, energy-esque values
	public float energy = 5.0f;

	public bool active = false;
	
	public CartController parent;

	public virtual void Use()
	{

	}

	public virtual void Fire(bool on)
	{

	}
}
