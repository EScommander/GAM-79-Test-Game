using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour 
{
	public string name = "Powerup";
	public float[] chancesPerPlace;
	public GameObject prefab;

	public enum e_connectionTypes{LEFT, RIGHT, TOP, BACK};
	public e_connectionTypes connectionType;

	//this holds all damage, shields, power-esque values
	public float power = 1.0f;

	//this holds all health, time, ammo, energy, energy-esque values
	public float energy = 1.0f;

	public virtual void Use()
	{

	}
}
