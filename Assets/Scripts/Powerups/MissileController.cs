using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour 
{
	public GameObject mainTarget;
	public Vector3 currentTarget;

	public float flightSpeed = 10.0f;
	public float turnSpeed = 10.0f;

	public float collisionDetectionRange = 100.0f;
	public float cornerPadding = 1.0f;

	public float explosionRadius = 3.0f;

	[ContextMenu("Randomize Node")]
	public void RandomizeNode()
	{
		this.mainTarget = TrackManager.SceneInstance.trackNodes[Mathf.FloorToInt( TrackManager.SceneInstance.trackNodes.Length * Random.value)];
	}

	public void Update()
	{
		if(Vector3.Distance(this.transform.position, this.mainTarget.transform.position) <= this.explosionRadius)
		{
			//BOOM
		}
		else
		{
			this.transform.rotation = Quaternion.Lerp (this.transform.rotation, Quaternion.LookRotation (this.currentTarget - this.transform.position), this.turnSpeed * Time.deltaTime);
			
			if(Vector3.Distance(this.transform.position, this.currentTarget) >= this.flightSpeed * Time.deltaTime)
			{
				this.transform.position += this.transform.forward * this.flightSpeed * Time.deltaTime;
			}
			else
			{
				this.transform.position = this.currentTarget;
			}
			
			RaycastHit hit;
			
			if(Physics.Raycast(this.transform.position, this.transform.forward, out hit, this.collisionDetectionRange))
			{
				if(hit.collider != null)
				{
					if(Vector3.Angle(hit.normal, this.transform.forward) > 90.0f)
					{
						if(Vector3.Angle(Vector3.up, this.transform.up) > 90.0f)
						{
							this.currentTarget = hit.collider.ClosestPointOnBounds(this.transform.position - (this.transform.forward * this.cornerPadding) - (this.transform.right * this.cornerPadding) - (this.transform.up * this.cornerPadding));
						}
						else
						{
							this.currentTarget = hit.collider.ClosestPointOnBounds(this.transform.position - (this.transform.forward * this.cornerPadding) - (this.transform.right * this.cornerPadding) + (this.transform.up * this.cornerPadding));
						}
					}
					else
					{
						if(Vector3.Angle(Vector3.up, this.transform.up) > 90.0f)
						{
							this.currentTarget = hit.collider.ClosestPointOnBounds(this.transform.position - (this.transform.forward * this.cornerPadding) + (this.transform.right * this.cornerPadding) - (this.transform.up * this.cornerPadding));
						}
						else
						{
							this.currentTarget = hit.collider.ClosestPointOnBounds(this.transform.position - (this.transform.forward * this.cornerPadding) + (this.transform.right * this.cornerPadding) + (this.transform.up * this.cornerPadding));
						}
					}
				}
			}
			else
			{
				if(this.mainTarget != null)
				{
					this.currentTarget = this.mainTarget.transform.position;
				}
			}
		}
	}
}
	