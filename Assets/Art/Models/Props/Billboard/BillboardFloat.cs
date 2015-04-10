using UnityEngine;
using System.Collections;

public class BillboardFloat : MonoBehaviour 
{
	public bool up = false;
	void Update () 
	{
		if (up == false) 
		{
			for (int i=0; i<100; i++)
			{
				this.transform.Translate (0, Time.deltaTime, 0);
			}
			up = true;
		}
		else
		{
			for (int i=0; i<100; i++)
			{
				this.transform.Translate (0, Time.deltaTime, 0);
			}
			up = false;
		}
		
	}
}
