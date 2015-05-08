using UnityEngine;
using System.Collections;

public class BThrustersAnimation : MonoBehaviour 
{
	private float time = 0;
	private int i = 0;

	public GameObject[] images;
	public float frame;


	void Update ()
	{
		time = time + Time.deltaTime;

		if (time > frame) {
			images [i].SetActive (true);
			if (i == 3) {
				images [i - 1].SetActive (false);
			} else if (i > 0) {
				images [i - 1].SetActive (false);
			} else if (i == 0) {
				images [3].SetActive (false);
			}
			time = 0;
			i++;

			if (i == 4)
				i = 0;
		}
	}


}

