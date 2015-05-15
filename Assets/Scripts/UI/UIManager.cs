using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour 
{


	GameObject cart = null;
	int selectedIndex = 0;
	int prevIndex = -1;

	float offset = 0.374f;
	float spinSpeed = 30.0f;

	public RectTransform panel = null;
	public GameObject cartButton = null;

	public static UIManager _instance = null;

	public CameraMove sceneCamera = null;

	GameObject[] racers;
	private List<GameObject> characterButtons = new List<GameObject>();

	[HideInInspector]
	public int buttonSelected = -1;

	private bool zoomed = false;

	public GameObject tagObj = null;

	EventSystem eventSystem;

	public static UIManager GetInstance()
	{
		//netManagerRef = NetworkManager.GetInstance ();

		if (_instance == null) 
		{
			GameObject go = new GameObject("_UIManager");
			_instance = (UIManager)go.AddComponent<UIManager>();
		}
	
		return _instance;
	}

	// Use this for initialization
	void Awake () 
	{
		eventSystem = EventSystem.current;
		_instance = this;

		Object[] objects = Resources.LoadAll("Carts");
		racers = new GameObject[objects.Length];
		for(int i = 0; i<objects.Length; i++)
		{
			//Cart GameObjects
			racers[i] = (GameObject)objects[i];

			//cart Buttons
			GameObject temp = Instantiate (cartButton);
			temp.transform.SetParent(panel.transform);
			temp.transform.name = racers [i].name;
			
			characterButtons.Add(temp);
			buttonSelected = 0;
			
			temp.GetComponent<RectTransform>().localScale = Vector3.one;
			
			PlayerData racerTempData = racers [i].GetComponent<PlayerData> ();					
			if (racerTempData != null)
				temp.transform.GetComponent<Image>().sprite = racers [i].GetComponent<PlayerData> ().characterSprite;
		}

		//disables auto navigation on UI elements
		eventSystem.sendNavigationEvents = false;
	}

	public void SwitchKart(string name)
	{
		for(int i = 0; i < racers.Length; i++)
		{
			if(racers[i].name == name)
			{
				GameObject prevCart = cart;
				cart = (GameObject)Instantiate(racers[i], Vector3.zero + Vector3.up * offset, prevCart.transform.rotation);
				cart.name = racers[i].name;
				Destroy (prevCart);
				buttonSelected = i;
				tagObj.transform.GetChild(0).GetComponent<Image>().sprite = racers[i].GetComponent<PlayerData>().characterSprite;
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		switch(NetworkManager.GetInstance().current)
		{
		case NetworkManager.e_NetworkMode.CHARACTER_SELECT:

			// Adding  characterButtons.Count because modulus of a negative gives negative numbers
			if(Input.GetKeyUp(KeyCode.D))  // go right in button list
			{
				buttonSelected = (buttonSelected + characterButtons.Count + 1) % (characterButtons.Count);
				SwitchKart(characterButtons[buttonSelected].name);
			}
			else if(Input.GetKeyUp(KeyCode.A)) // go left in button list
			{
				buttonSelected = (buttonSelected + characterButtons.Count -1) % (characterButtons.Count);
				SwitchKart(characterButtons[buttonSelected].name);
			}
			else if(Input.GetKeyUp(KeyCode.S))
			{
				buttonSelected = (buttonSelected + characterButtons.Count + 3) % (characterButtons.Count);
				SwitchKart(characterButtons[buttonSelected].name);
			}
			else if(Input.GetKeyUp(KeyCode.W))
			{
				buttonSelected = (buttonSelected + characterButtons.Count - 3) % (characterButtons.Count);
				SwitchKart(characterButtons[buttonSelected].name);
			}

			if(Input.GetKeyUp(KeyCode.Z))
			{
				zoomed = !zoomed;
			}
			
			if(Input.GetKeyUp(KeyCode.Space))
			{
				// advance to race or track select
			}

			eventSystem.SetSelectedGameObject(characterButtons[buttonSelected]);

			if(cart != null)
				cart.transform.Rotate (Vector3.up * spinSpeed * Time.deltaTime);
			else 
			{
				cart = (GameObject)Instantiate(racers[0], Vector3.zero + Vector3.up * offset, racers[0].transform.rotation);
				cart.name = racers[0].name;
				prevIndex = 0;
			}

			if(zoomed)
				sceneCamera.SetPosition(cart.GetComponent<PlayerData>().zoomedView);
			else sceneCamera.SetPosition(cart.GetComponent<PlayerData>().regularView);

			break;

			case NetworkManager.e_NetworkMode.MAP_SELECT:
				break;

			case NetworkManager.e_NetworkMode.RACE:
				break;
		}
	

	}

	void OnGUI()
	{

	}

	public void SelectCharacter()
	{
		NetworkManager.GetInstance().setRacer(cart.GetComponent<PlayerData>().cartPrefab.gameObject);
	}

	public void StartRace()
	{
		SelectCharacter ();
	}
}
