using UnityEngine;
using System.Collections;

public class CasmeraModTunnle : MonoBehaviour {

	private  CartController cartController = null;

	public float normalAngle = 60;
	public float tunnleAngle = 115;

	private float lerpVal = 0;

	// Use this for initialization
	void Start ()
	{
		cartController = GetComponent<CartController> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		UpdateAnimations ();
		UpdateCameraFOV ();
	}

	void UpdateAnimations()
	{
		Animator anim = cartController.cartChar.GetComponent<Animator> ();
		anim.SetFloat ("SpeedInput", lerpVal);
	}

	void UpdateCameraFOV()
	{
		float speed = cartController.Speedometer;
		float maxSpeed = cartController.topSpeed;
		
		lerpVal = speed / maxSpeed;
		
		Camera.main.fieldOfView = Mathf.Lerp (normalAngle, tunnleAngle, lerpVal);
		
		Debug.Log(lerpVal);
	}
}
