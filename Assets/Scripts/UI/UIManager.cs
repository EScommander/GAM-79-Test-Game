using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

	public RectTransform panel = null;
	public GameObject cartButtonParent = null;
	private Button cartButton = null;

	private RectTransform buttonRT;
	private float buttonHeight = 0f;
	private float buttonWidth  = 0f;

	private int iconsPerColumn = 3;
	private float pixelSpacing = 20.0f;

	private bool haveButtons = false;

	public static UIManager _instance = null;
	public static UIManager GetInstance()
	{
		if (_instance == null) 
		{
			GameObject go = new GameObject("_UIManager");
			_instance = (UIManager)go.AddComponent<UIManager>();
		}
	
		return _instance;
	}

	// Use this for initialization
	void Start () 
	{
		_instance = this;

		cartButton = cartButtonParent.transform.GetChild(0).GetComponent<Button>();

		Object[] objects = Resources.LoadAll("Carts");
		racers = new GameObject[objects.Length];
		for(int i = 0; i<objects.Length; i++)
		{
			racers[i] = (GameObject)objects[i];
			racers[i].GetComponent<NetworkSyncedCart>().enabled = false;
			//racers[i].GetComponent<CartController>().enabled = false;
		}
		prevScreen = screenStage.START;

		buttonRT = cartButton.GetComponent<RectTransform> ();

		buttonHeight = buttonRT.rect.height;
		buttonWidth = buttonRT.rect.width;
	}

	public void SwitchKart(string name)
	{
		Debug.Log (racers.Length);
		for(int i = 0; i < racers.Length; i++)
		{
			if(racers[i].name == name)
			{
				Debug.Log (prevIndex);
				if(prevIndex != -1)
				{
					GameObject prevCart = cart;
					cart = (GameObject)Instantiate(racers[i], Vector3.zero + Vector3.up * offset, prevCart.transform.rotation);
					Destroy (prevCart);
					prevIndex = i;
				}
				else
				{
					cart = (GameObject)Instantiate(racers[i], Vector3.zero + Vector3.up * offset, racers[i].transform.rotation);
					prevIndex = i;
				}
			}

		}
	}

	// Update is called once per frame
	void Update () 
	{		
		if (!haveButtons) 
		{
			haveButtons = true;
			Debug.Log (racers.Length);
			Debug.Log ("buttons");
			for (int i = 0; i < racers.Length; i++) {
				int row = (int)i / iconsPerColumn;
				int column = i % iconsPerColumn;
				GameObject temp = (GameObject)Instantiate (cartButtonParent);//, panel.anchorMax, Quaternion.identity);
				temp.transform.parent = panel.transform;
				temp.transform.GetChild(0).name = racers [i].name;
				
				PlayerData racerTempData = racers [i].GetComponent<PlayerData> ();
				
				if (racerTempData != null)
					temp.transform.GetChild(0).GetComponent<Image>().sprite = racers [i].GetComponent<PlayerData> ().characterSprite;
				
				temp.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (panel.anchorMin.x + (buttonWidth * 0.8f * column) + pixelSpacing, 
				                                                                   panel.anchorMin.y - (buttonHeight * (row * 0.8f + 0.5f)));
			}
		}

		if(cart != null)
			cart.transform.Rotate (Vector3.up * spinSpeed * Time.deltaTime);
		else 
		{
			cart = (GameObject)Instantiate(racers[0], Vector3.zero + Vector3.up * offset, racers[0].transform.rotation);
			prevIndex = 0;
		}
	}

	public void StartRace()
	{
		DontDestroyOnLoad(gameObject);
		Application.LoadLevel("city_scaled");
		currentScreen = screenStage.PLAY;
	}
}
