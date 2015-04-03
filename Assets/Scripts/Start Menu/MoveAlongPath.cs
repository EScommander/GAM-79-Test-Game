using UnityEngine;
using System.Collections;

public class MoveAlongPath : MonoBehaviour 
{
	public float moveSpeed = 50f;

	// Use this for initialization
	void Start () 
	{
		iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("CameraMove"), "orienttopath", true, "speed", moveSpeed, "looptype", "loop", "easetype", "Linear"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
