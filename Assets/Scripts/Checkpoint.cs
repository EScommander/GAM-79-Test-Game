using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
[RequireComponent (typeof (BoxCollider))]
public class Checkpoint : MonoBehaviour 
{
	public Collider collider;
	public Vector3 trackForward;
	public int checkpointOrderIndex = 0;

	private void Start()
	{
		if (!Application.isPlaying)
			return;

		this.collider = gameObject.GetComponent<BoxCollider> ();
		this.collider.isTrigger = true;
		this.trackForward = this.transform.forward;
	}

	private void Update()
	{
		if (Application.isPlaying)
			return;
		
		Debug.DrawRay(this.transform.position, this.transform.forward * 3.0f);
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.LogError (other.name + " Entered Checkpoint Index " + this.checkpointOrderIndex);

		if (!Application.isPlaying)
			return;

		bool wrongWay = false;

		if(other != null && other.gameObject != null)
		{
			Rigidbody otherRigidbody = other.gameObject.GetComponent<Rigidbody>();

			if(otherRigidbody != null)
			{
				if((otherRigidbody.velocity.normalized + this.trackForward.normalized).magnitude <= this.trackForward.normalized.magnitude)
				{
					wrongWay = true;
				}
			}

			CartLapController otherLapController = other.GetComponent<CartLapController>();

			if(otherLapController != null)
			{
				if(wrongWay)
				{
					otherLapController.wrongWay = true;
				}
				else
				{
					otherLapController.EnteredCheckpoint(this.checkpointOrderIndex);
				}
			}
		}
	}
}
