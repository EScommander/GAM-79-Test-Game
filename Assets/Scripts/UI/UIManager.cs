﻿using UnityEngine;
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

	private int currentRow = 1;
	private int maxRow = 4;
	private int charsPerRow = 3;
	private bool dPadEnabled = true;

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

		tagObj.transform.GetComponent<Tag>().charSprite.sprite = racers[0].GetComponent<PlayerData>().characterSprite;
		tagObj.transform.GetComponent<Tag>().characterName.text = racers[0].GetComponent<PlayerData>().name;
		tagObj.transform.GetComponent<Tag>().status.text = "Choosing...";

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
				tagObj.transform.GetComponent<Tag>().charSprite.sprite = racers[i].GetComponent<PlayerData>().characterSprite;
				tagObj.transform.GetComponent<Tag>().characterName.text = racers[i].GetComponent<PlayerData>().name;
				tagObj.transform.GetComponent<Tag>().status.text = "Choosing...";
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		switch(NetworkManager.GetInstance().current)
		{
		case NetworkManager.e_NetworkMode.CHARACTER_SELECT:
			// keyboard and joystick inputs
			if(Input.GetAxis("D-PadLR") == 1 && dPadEnabled)         //move right
			{
				dPadEnabled = false;
				StartCoroutine(DPadInput(1, false));

			}
			else if(Input.GetAxis("D-PadLR") == -1 && dPadEnabled)   //move left
			{
				dPadEnabled = false;
				StartCoroutine(DPadInput(-1, false));
			}

			if(Input.GetAxis("D-PadUD") == 1 && dPadEnabled)         //move up
			{
				dPadEnabled = false;
				StartCoroutine(DPadInput(1, true));

			}
			else if(Input.GetAxis("D-PadUD") == -1 && dPadEnabled)   //move down
			{
				dPadEnabled = false;
				StartCoroutine(DPadInput(-1, true));

			}

			//zoom
			if(Input.GetButtonDown ("Zoom"))
			{
				zoomed = !zoomed;
			}
			
			if(Input.GetButtonDown("Submit"))
			{
				StartRace();
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

	public IEnumerator DPadInput(int value, bool isVertical)
	{
		if(!isVertical)
		{
			if(value == 1)  // go right in button list
			{
				if(buttonSelected + 1 < racers.Length-1)
				{
					buttonSelected = (buttonSelected + 1) % 3 + charsPerRow*(currentRow-1);//(buttonSelected + (charsPerRow * currentRow) + 1) % (charsPerRow * currentRow);
					SwitchKart(characterButtons[buttonSelected].name);
				}
				else
				{
					buttonSelected = charsPerRow * (currentRow-1);
					SwitchKart(characterButtons[buttonSelected].name);
				}
			}
			else // go left in button list
			{
				if((buttonSelected + 2) % 3 + charsPerRow*(currentRow-1) < racers.Length-1)
				{
					buttonSelected = (buttonSelected + 2) % 3 + charsPerRow*(currentRow-1); //(buttonSelected + (charsPerRow * currentRow) -1) % (charsPerRow * currentRow);
					SwitchKart(characterButtons[buttonSelected].name);
				}
				else
				{
					buttonSelected = racers.Length -1;
					SwitchKart(characterButtons[buttonSelected].name);
				}
			}
		}
		else
		{
			if(value == -1)
			{
				if(buttonSelected + 3 > racers.Length-1)
				{
					buttonSelected = (buttonSelected + 3) % 3;
					SwitchKart(characterButtons[buttonSelected].name);
					currentRow = 1;
				}
				else
				{
					buttonSelected = (buttonSelected + 3) % (charsPerRow * maxRow);//(buttonSelected + characterButtons.Count + 3) % (characterButtons.Count);
					SwitchKart(characterButtons[buttonSelected].name);
					currentRow++;
				}
			}
			else
			{
				if((buttonSelected + (charsPerRow * maxRow) - 3) % (charsPerRow * maxRow)  > racers.Length -1)
				{
					buttonSelected = (buttonSelected + (charsPerRow * maxRow) - 6) % (charsPerRow * maxRow);
					SwitchKart(characterButtons[buttonSelected].name);
					currentRow = maxRow -1;
				}
				else
				{
					buttonSelected = (buttonSelected + (charsPerRow * maxRow) - 3) % (charsPerRow * maxRow);
					SwitchKart(characterButtons[buttonSelected].name);
					if(currentRow - 1 < 1)
						currentRow = maxRow;
					else currentRow--;
				}
			}
		}

		yield return new WaitForSeconds(0.3f);
		dPadEnabled = true;
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
