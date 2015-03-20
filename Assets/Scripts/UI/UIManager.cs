using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour 
{

	GameObject[] racers;
	int selectedIndex = 0;
	int prevIndex = -1;
	GameObject cart = null;

	float offset = 0.374f;
	float spinSpeed = 30.0f;

	public enum screenStage {START, CHAR_SEL, TRACK_SEL, PLAY};
	screenStage currentScreen = screenStage.CHAR_SEL;
	screenStage prevScreen;

	public string LevelToLoad = "";


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
		prevScreen = screenStage.START;
	}

	void OnGUI()
	{
		switch (currentScreen) 
		{
		case screenStage.CHAR_SEL:
			GUI.BeginScrollView (new Rect (100, 100, 200, 400), Vector2.zero, new Rect (0, 0, 200, 400));
			for (int i = 0; i < racers.Length; i++) {
				if (GUILayout.Button (racers [i].name, GUILayout.MinWidth (150), GUILayout.MinHeight (100)))
					selectedIndex = i;
			}
			GUI.EndScrollView ();
			if(GUI.Button(new Rect(Screen.width - 300, Screen.height - 200, 200, 100), "Race!"))
			{
				DontDestroyOnLoad(gameObject);
				Application.LoadLevel("_Debug_TrackTest1");
				currentScreen = screenStage.PLAY;
			}

			break;
		}


	}

	// Update is called once per frame
	void Update () 
	{
		switch(currentScreen)
		{
//
//		case screenStage.START:
//			currentScreen = screenStage.CHAR_SEL;
//			break;
		case screenStage.CHAR_SEL:
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
			break;
//		case screenStage.TRACK_SEL:
//			currentScreen = screenStage.PLAY;
//			break;
//		case screenStage.PLAY:
//			if(prevScreen != screenStage.PLAY)
//			{
//				Application.LoadLevel("_Debug_TrackTes1");
			}
	}
}
