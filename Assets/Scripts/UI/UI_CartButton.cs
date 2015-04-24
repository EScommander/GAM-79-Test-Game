using UnityEngine;
using System.Collections;

public class UI_CartButton : MonoBehaviour 
{
	public void SwitchKart()
	{
		Debug.Log (gameObject.name);
		UIManager.GetInstance ().SwitchKart (gameObject.name);
	}
}
