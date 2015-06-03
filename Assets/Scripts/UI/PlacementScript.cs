using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlacementScript : MonoBehaviour 
{
	private static PlacementScript _instance;

	public Text text; 

	public static void UpdatePlace(int place)
	{
		if(PlacementScript._instance != null)
		{
			PlacementScript._instance.UpdatePlaceInstance(place);
		}
	}

	private void Start()
	{
		if(PlacementScript._instance == null)
		{
			PlacementScript._instance = this;
		}

		if (this.text == null) 
		{
			this.text = this.gameObject.GetComponent<Text>();
		}
	}

	public void UpdatePlaceInstance(int place)
	{
		if (this.text == null) 
		{
			this.text = this.gameObject.GetComponent<Text>();
		}

		if (this.text != null)
		{
			if(place == 1)
			{
				this.text.text = "1st";
			}
			else if(place == 2)
			{
				this.text.text = "2nd";
			}
			else if(place == 3)
			{
				this.text.text = "3rd";
			}
			else
			{
				this.text.text = place + "th";
			}
		}
	}
}
