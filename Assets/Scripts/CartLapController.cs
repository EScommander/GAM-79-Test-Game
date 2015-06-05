using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CartLapController : MonoBehaviour 
{
	public static int numLaps = 2;

	public int currentLap = 0;
	public bool wrongWay = false;
	public int currentCheckpoint = 0;
	public float distanceToNext = 0.0f;
	public bool finishedRace = false;

	public int currentPlace = 1;

	private NetworkView myView;

	public void OnFinishRace()
	{
		this.finishedRace = true;
	}

	void Start()
	{
		if(UIInGame.SceneInstance.lapCount != null)
		{
			UIInGame.SceneInstance.lapCount.text = (this.currentLap+1)+"/"+CartLapController.numLaps;
			UIInGame.SceneInstance.lapCount.gameObject.SetActive(true);
		}
	}

	private void Update()
	{
		if(myView == null)
		{
			myView = transform.GetComponent<NetworkView> ();
		}

		if(myView != null && myView.isMine)
		{
			if(currentCheckpoint + 1 < CartRacer.TrackManager.SceneInstance.checkpoints.Length)
			{
				this.distanceToNext = Vector3.Distance(this.transform.position, CartRacer.TrackManager.SceneInstance.checkpoints[currentCheckpoint].transform.position);
			}
			else if(CartRacer.TrackManager.SceneInstance.checkpoints.Length > 0)
			{
				this.distanceToNext = Vector3.Distance(this.transform.position, CartRacer.TrackManager.SceneInstance.checkpoints[0].transform.position);
			}

			CartLapController[] lapControllers = GameObject.FindObjectsOfType<CartLapController> ();

			List<CartLapController> tiedControllers = new List<CartLapController>();

			int previousPlace = this.currentPlace;

			int placing = 1;

			for(int i = 0; i < lapControllers.Length; i++)
			{
				if(lapControllers[i] == this)
				{
					continue;
				}

				if(lapControllers[i].currentLap > this.currentLap)
				{
					placing++;
				}
				else if(lapControllers[i].currentLap == this.currentLap && lapControllers[i].currentCheckpoint > this.currentCheckpoint)
				{
					placing++;
				}
				else if(lapControllers[i].currentLap == this.currentLap && lapControllers[i].currentCheckpoint == 0 && this.currentCheckpoint != 0)
				{
					placing++;
				}
				else if(lapControllers[i].currentLap == this.currentLap && lapControllers[i].currentCheckpoint == this.currentCheckpoint && lapControllers[i].distanceToNext < this.distanceToNext)
				{
					placing++;
				}
			}

			this.currentPlace = placing;

			//if(this.currentPlace != previousPlace)
			//{
			if(UIInGame.SceneInstance.racePostion != null)
				UIInGame.SceneInstance.racePostion.gameObject.SetActive(true);
			PlacementScript.UpdatePlace(this.currentPlace);
			//}
		}
	}

	public void EnteredCheckpoint(int position)
	{
		if(myView != null && myView.isMine)
		{
			if(position == currentCheckpoint)
			{
				if(currentCheckpoint + 1 >= CartRacer.TrackManager.SceneInstance.checkpoints.Length)
				{
					currentCheckpoint = 0;
				}
				else if(currentCheckpoint == 0)
				{
					currentCheckpoint = 0;
					currentLap++;
					
					UIInGame.SceneInstance.lapCount.text = (this.currentLap)+"/"+CartLapController.numLaps;
					
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
}
