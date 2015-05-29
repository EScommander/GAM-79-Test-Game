using UnityEngine;
using System.Collections;

public class ToggleBigHeadMode : MonoBehaviour 
{
	public static void BigHeadMode(bool enable)
	{
		GameObject[] gos = GameObject.FindGameObjectsWithTag("Head");

		foreach(GameObject go in gos)
		{
			if(go != null)
			{
				if(enable)
				{
					go.transform.localScale = new Vector3(3.0f,3.0f,3.0f);
				}
				else
				{
					go.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
				}
			}
		}
	}
}
