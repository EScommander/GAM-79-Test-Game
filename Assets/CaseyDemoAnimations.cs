using UnityEngine;
using System.Collections;

public class CaseyDemoAnimations : MonoBehaviour {
	public float TurnInput = 0;
	public float SpeedInput = 0;
	float timeTo = 5;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		Animator anim = this.GetComponent<Animator> ();

		if (Time.time > 1+timeTo && Time.time < 3.5+timeTo)
			TurnInput = Mathf.Lerp (TurnInput, Mathf.Sin ((Time.time - (1+timeTo)) * 2), 0.2f);
		else
			TurnInput = Mathf.Lerp (TurnInput, 0, 0.2f);

		anim.SetFloat ("TurnInput",  TurnInput);
		anim.SetFloat ("SpeedInput", 1);
	}
}
