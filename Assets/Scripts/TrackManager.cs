using UnityEngine;
using System.Collections;

public class TrackManager : MonoBehaviour 
{
	public static TrackManager SceneInstance
	{
		get
		{
			if(TrackManager._instance == null)
			{
				GameObject go = new GameObject("TrackManager");
				TrackManager._instance = go.AddComponent<TrackManager>();
			}

			return TrackManager._instance;
		}
		set
		{
			if(TrackManager._instance == null)
			{
				TrackManager._instance = value;
			}
		}
	}

	private static TrackManager _instance;

	public GameObject[] trackNodes;

	public Vector3 nearestNode(Vector3 fromPos)
	{
		Vector3 chosenNode = fromPos;

		if(trackNodes.Length > 0)
		{
			chosenNode = trackNodes[0].transform.position;
		}

		for(int i = 0 ; i < trackNodes.Length; i++)
		{
			if(Vector3.Distance(fromPos, chosenNode) >= Vector3.Distance(fromPos, trackNodes[i].transform.position))
			{
				chosenNode = trackNodes[i].transform.position;
			}
		}

		return chosenNode;
	}
}
