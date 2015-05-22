// MyScriptEditor.cs
using UnityEditor;
using UnityEngine;
//using System.Collections;


//[CustomEditor(typeof(MyScript))] 
public class RotateToNormalEditor :  MonoBehaviour 
{
	[MenuItem ("ckTools/Snap to Normal %u")]
	public static void SnapToNormal()
	{
		GameObject[] List = Selection.gameObjects;

		for(int i=0; i<List.Length; i++)
		{
			Transform mySelection = List[i].transform;//Selection.activeGameObject.transform;

			Debug.Log ("Selecting - " + mySelection);

			Ray ray = new Ray (mySelection.position, mySelection.up * (-1));
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit))
			{
				Debug.Log ("Hit - " + hit.normal);
				//mySelection.rotation = Quaternion.Euler(0,0,0)*hit.normal;
				mySelection.rotation = Quaternion.FromToRotation (mySelection.up, hit.normal) * mySelection.rotation;
				mySelection.position = (hit.point);

				//mySelection.Rotate(mySelection.right, 90);// LookRotation(hit.normal)
				Debug.DrawLine (hit.point, (hit.point + hit.normal), Color.red);

			}
		}
	}


	[MenuItem ("ckTools/Move TM NodesToSnap %t")]
	public static void MoveTrackmanagerNodesToSnap()
	{
		GameObject[] List = Selection.gameObjects;
		
		for(int i=0; i<List.Length; i++)
		{
			Transform mySelection = List[i].transform;//Selection.activeGameObject.transform;
			
			Debug.Log ("Selecting - " + mySelection);
			
			Ray ray = new Ray (mySelection.position, mySelection.up * (-1));
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit))
			{
				Debug.Log ("Hit - " + hit.normal);
				//mySelection.rotation = Quaternion.Euler(0,0,0)*hit.normal;
				mySelection.rotation = Quaternion.FromToRotation (mySelection.up, hit.normal) * mySelection.rotation;
				mySelection.position = (hit.point + hit.normal*1.2f);
				//mySelection.Rotate(mySelection.right, 90);// LookRotation(hit.normal)
				Debug.DrawLine (hit.point, (hit.point + hit.normal), Color.red);
				
			}
		}
	}

	public static void OnDrawGizmos()
	{



	}
	//public override void OnInspectorGUI()
	//{
	//
	//	Debug.Log ("HELLO");
	//	SceneView.RepaintAll();
	//}
}




