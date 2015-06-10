using UnityEngine;
using System.Collections;

public class MapIconPosition : MonoBehaviour 
{

	private float yPos = 600f;
	public GameObject objectToFollow = null;

	public GameObject dir = null;

	private bool rotationSet = false;

	// Use this for initialization
	void Start () 
	{
	
	}

	// Update is called once per frame
	void Update () 
	{
		if(!rotationSet && objectToFollow != null)
		{
			Debug.Log ("Setting rotation " + transform.eulerAngles.y + ": " + objectToFollow.transform.eulerAngles.y );

			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, objectToFollow.transform.eulerAngles.y - 180f, transform.localEulerAngles.z);
			rotationSet = true;
		}
		
		if(objectToFollow != null)
		{
			if(dir != null)
				dir.transform.eulerAngles = new Vector3(dir.transform.eulerAngles.x, objectToFollow.transform.eulerAngles.y - 180f, dir.transform.eulerAngles.z);
			transform.position = new Vector3(objectToFollow.transform.position.x, yPos, objectToFollow.transform.position.z);
		}
	}

	public void SetIconScale(float scale)
	{
		transform.localScale = scale * transform.localScale;
		dir.SetActive(false);
	}
}
