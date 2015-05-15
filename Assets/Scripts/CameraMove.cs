using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour 
{

	private float distThreshold = 0.05f;
	private Vector3 updatedCameraPos = Vector3.zero;

	private float updateTime = 0.05f;

	void Awake()
	{
		updatedCameraPos = transform.position;
	}

	void Update()
	{
		if(Vector3.Distance(transform.position, updatedCameraPos) > distThreshold)
		{
			transform.position = Vector3.Lerp(transform.position, updatedCameraPos, updateTime);
		}
	}

	public void SetPosition(Vector3 pos)
	{
		updatedCameraPos = pos;
	}
}
