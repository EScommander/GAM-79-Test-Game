using UnityEngine;
using System.Collections;

public class CharacterSelect : MonoBehaviour 
{

	GameObject[] racers;
	int selectedIndex = 0;
	int prevIndex = -1;
	GameObject cart = null;

	float offset = 0.374f;
	float spinSpeed = 30.0f;


	// Use this for initialization
	void Start () 
	{
		Object[] objects = Resources.LoadAll("Carts");
		racers = new GameObject[objects.Length];
		for(int i = 0; i<objects.Length; i++)
		{
			racers[i] = (GameObject)objects[i];
			racers[i].GetComponent<NetworkSyncedCart>().enabled = false;
			racers[i].GetComponent<CartController>().enabled = false;
		}
	}

	void OnGUI()
	{
		GUI.BeginScrollView (new Rect(100,100, 200, 400), Vector2.zero, new Rect(0,0, 200, 400));
		for(int i = 0; i < racers.Length; i++)
		{
			if(GUILayout.Button (racers[i].name, GUILayout.MinWidth(150), GUILayout.MinHeight(100)))
				selectedIndex = i;
		}
		GUI.EndScrollView ();


	}

	// Update is called once per frame
	void Update () 
	{
		if(cart != null)
			cart.transform.Rotate (Vector3.up, Time.deltaTime * spinSpeed);

		if(prevIndex != selectedIndex)
		{
			Debug.Log (selectedIndex);
			if(prevIndex != -1)
				Destroy (cart);
			cart = (GameObject)Instantiate(racers[selectedIndex], Vector3.zero + Vector3.up * offset, racers[selectedIndex].transform.rotation);
			prevIndex = selectedIndex;
		}
	}
}
