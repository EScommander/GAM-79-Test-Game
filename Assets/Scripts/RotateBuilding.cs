using UnityEngine;
using System.Collections;

public class RotateBuilding : MonoBehaviour 
{
	float speed = 10.0f;
	float time = 0.0f;
	float rate = 1.5f;
	bool spin = false;
	int random = 0;
	int randomTime = 0;


	void Update () 
	{
		random = Random.Range (0, 50);
		randomTime = Random.Range (4, 5);

		if (random == 2) 
		{
			spin = true;
		}
		if (spin == true) 
		{
			time += Time.deltaTime;
		
			if (time <= 4.0f) 
			{
				transform.Rotate (Vector3.up, speed * Time.deltaTime);
			}
			if (time >= randomTime) 
			{
				speed = Random.Range(0,1);
				if (speed == 0)
				{
					speed = -10.0f;
				}
				else 
				{
					speed = 10.0f;
				}
				time = 0.0f;
				spin = false;
			}
		}
	}
}
