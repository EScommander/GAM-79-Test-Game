using UnityEngine;
using System.Collections;

public class CartLapController : MonoBehaviour 
{
	public static int numLaps = 2;

	public int currentLap = 0;
	public bool wrongWay = false;
	public int currentCheckpoint = 0;
	public bool finishedRace = false;

	public void OnFinishRace()
	{
		this.finishedRace = true;
	}

	public void EnteredCheckpoint(int position)
	{
		if(position == currentCheckpoint)
		{
			if(currentCheckpoint + 1 >= CartRacer.TrackManager.SceneInstance.checkpoints.Length)
			{
				currentCheckpoint = 0;
				currentLap++;

				if(currentLap > CartLapController.numLaps)
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
