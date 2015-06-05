using UnityEngine;
using System.Collections;

namespace CartRacer
{
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

		private void Start()
		{
			TrackManager.SceneInstance = this;

			/*if(checkpoints != null && checkpoints.Length > 0)
			{
				for(int i = 0 ; i < checkpoints.Length; i++)
				{
					if(checkpoints[i] != null)
					{
						checkpoints[i].checkpointOrderIndex = i;
					}
				}
			}*/
		}

		public GameObject[] trackNodes;
		public Checkpoint[] checkpoints;

		public GameObject nearestNode(Vector3 fromPos)
		{
			GameObject chosenNode = null;// = fromPos;

			if(trackNodes.Length > 0)
			{
				chosenNode = trackNodes[0];
			}

			for(int i = 0 ; i < trackNodes.Length; i++)
			{
				if(Vector3.Distance(fromPos, chosenNode.transform.position) >= Vector3.Distance(fromPos, trackNodes[i].transform.position))
				{
					chosenNode = trackNodes[i];
				}
			}

			return chosenNode;
		}
	}
}
