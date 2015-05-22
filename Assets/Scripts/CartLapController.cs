using UnityEngine;
using System.Collections;

public class CartLapController : MonoBehaviour 
{
	public static int numLaps = 1;

	public int currentLap = 1;
	public bool wrongWay = false;
	public int currentCheckpoint = 1;
	public bool finishedRace = false;

	public void OnFinishRace()
	{
		this.finishedRace = true;
		Debug.LogError("FINISHED RACE AS #" + CartRacer.TrackManager.numCartsFinshed + " CART!");
		CartRacer.TrackManager.numCartsFinshed++;
		WInText.instance.text.text = "Place: " + CartRacer.TrackManager.numCartsFinshed;
	}

	public void EnteredCheckpoint(int position)
	{
		if(!this.finishedRace && position == currentCheckpoint)
		{
			if(currentCheckpoint + 1 >= CartRacer.TrackManager.SceneInstance.checkpoints.Length)
			{
				currentCheckpoint = 0;
				currentLap++;

				if(currentLap >= CartLapController.numLaps)
				{
					this.OnFinishRace();
				}
			}
			else
			{
				currentCheckpoint++;
			}
		}
	}
}
