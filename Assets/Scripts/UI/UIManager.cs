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

	public List<GameObject> serverButtons = new List<GameObject>();
	public List<GameObject> trackButtons = new List<GameObject>();

	[HideInInspector]
	public int buttonSelected = -1;

	private bool zoomed = false;

	public GameObject tagObj = null;

	private int currentRow = 1;
	private int maxRow = 4;
	private int charsPerRow = 3;
	private bool dPadEnabled = true;

	EventSystem eventSystem;

	public GameObject serverUIPrefab = null;

	public enum e_UISubscreen {JOIN, CREATE, LAYOUT};
	public e_UISubscreen current = e_UISubscreen.LAYOUT;

	public GameObject layoutDisplay = null;
	public GameObject lobbyMenu = null;
	public GameObject joinMenu = null;
	public GameObject createMenu = null;
	public GameObject charSelectMenu = null;
	public GameObject charGrid = null;

	public List<Tag> tags = new List<Tag>();
	public GameObject tagGrid = null;
	public GameObject tagPrefab = null;


	public GameObject waitingForPlayers = null;
	public Button serverStartRace = null;

	public GameObject serverList = null;
	public GameObject levelSelectList = null;
	public Text emptyServerList = null;

	private bool selectPlayerNum = false;
	private bool enableChangePlayerCount = true;
	private int minPlayers = 1;
	public Text minPlayersLabel = null;

	public Button minPlayersButton = null;
	public Button incPlayers = null;
	public Button decPlayers = null;

	public GameObject trackUIPrefab = null;

	public List<TrackData> tracks = new List<TrackData>();

	private int scrolledToPos = 0;
	public RectTransform levelScrollView;

	public AudioSource clickSound = null;
	public AudioSource incorrectClick = null;
	public AudioSource confirmSound = null;
	public AudioSource charSelectSound = null;

	public Toggle bigHeadToggle = null;
	bool prevHeadMode = false;


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
		waitingForPlayers.SetActive(false);

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


		//populate list of tracks
		foreach(TrackData track in tracks)
		{
			GameObject levelUIObject = (GameObject)Instantiate (trackUIPrefab, levelSelectList.transform.position, levelSelectList.transform.rotation);
			levelUIObject.transform.SetParent(levelSelectList.transform);
			levelUIObject.GetComponentInChildren<MapSelectUI>().SetUIData(track);
			levelUIObject.GetComponent<RectTransform>().localScale = Vector3.one;

			trackButtons.Add (levelUIObject);
			//serverButtons.Add (serverUIObject);	
		}

		//disables auto navigation on UI elements
		eventSystem.sendNavigationEvents = false;

		//always defaults to joining a server
		SwapUI("Layout");
	}

	// Update is called once per frame
	void Update () 
	{
		switch(NetworkManager.GetInstance().current)
		{
		case NetworkManager.e_NetworkMode.SERVER_SELECT:
			if(current == e_UISubscreen.LAYOUT)
			{
				if(Input.GetButtonDown("Submit"))
				{
					SwitchToJoin();
				}
			}
			else if(current == e_UISubscreen.JOIN)
			{
				if(serverButtons.Count > 0)
				{ 
					emptyServerList.gameObject.SetActive(false);

					if(Input.GetAxis("D-PadUD") == 1 && dPadEnabled)         //move up
					{
						dPadEnabled = false;
						StartCoroutine(DPadInputServer(1));
						
					}
					else if(Input.GetAxis("D-PadUD") == -1 && dPadEnabled)   //move down
					{
						dPadEnabled = false;
						StartCoroutine(DPadInputServer(-1));
					}

					eventSystem.SetSelectedGameObject(serverButtons[buttonSelected]);
				}
				else emptyServerList.gameObject.SetActive(true);

				if(Input.GetButtonDown("CameraFlip"))
				{
					RefreshHostList();
				}

				if(Input.GetButtonDown("Fire1"))
				{
					SwitchToCreate ();
				}

				if(Input.GetButtonDown("Submit"))
				{
					if(serverButtons.Count > 0)
					{
						confirmSound.Play();
						JoinServer ();
					}
				}
			}
			else
			{
				minPlayersLabel.text = minPlayers.ToString();

				if(selectPlayerNum)
				{
					if(dPadEnabled) //gives small delay
						minPlayersButton.Select();
					if(Input.GetAxis("D-PadUD") == 1 && dPadEnabled)         //move up
					{
						//dPadEnabled = false;
						//StartCoroutine(DPadInputServer(1));
						incPlayers.Select();
						StartCoroutine(ChangePlayerReq(1));
						dPadEnabled = false;						
					}
					else if(Input.GetAxis("D-PadUD") == -1 && dPadEnabled)   //move down
					{
						//dPadEnabled = false;
						//StartCoroutine(DPadInputServer(-1));
						decPlayers.Select();
						StartCoroutine(ChangePlayerReq(-1));
						dPadEnabled = false;
					}

					if(Input.GetAxis("D-PadLR") == -1)
					{
						clickSound.Play();
						trackButtons[buttonSelected].GetComponentInChildren<Button>().enabled = true;
						selectPlayerNum = false;
					}

					if(Input.GetButtonDown("Submit"))
					{	
						confirmSound.Play();
						CreateServer();
					}
				}
				else if(trackButtons.Count > 0 )
				{
					trackButtons[buttonSelected].GetComponentInChildren<Button>().Select();
					if(Input.GetAxis("D-PadUD") == 1 && dPadEnabled)         //move up
					{
						dPadEnabled = false;
						StartCoroutine(DPadInputMaps(1));
						
					}
					else if(Input.GetAxis("D-PadUD") == -1 && dPadEnabled)   //move down
					{
						dPadEnabled = false;
						StartCoroutine(DPadInputMaps(-1));
					}

					if(Input.GetAxis("D-PadLR") == 1)
					{
						clickSound.Play();
						trackButtons[buttonSelected].GetComponentInChildren<Button>().enabled = false;
						selectPlayerNum = true;
						minPlayersButton.Select();
					}
					
					//eventSystem.SetSelectedGameObject(trackButtons[buttonSelected].transform.GetChild(0).gameObject);

					if(Input.GetButtonDown("Submit"))
					{
						selectPlayerNum = true;
						minPlayersButton.Select();
					}
				}

				if(Input.GetButtonDown("Fire1"))
				{
					SwitchToJoin();
				}
			}
			break;
		case NetworkManager.e_NetworkMode.CHARACTER_SELECT:
			if(bigHeadToggle.isOn != prevHeadMode)
			{
				prevHeadMode = bigHeadToggle.isOn;

				ToggleBigHeadMode.BigHeadMode(bigHeadToggle.isOn);
			}

			// keyboard and joystick inputs
			if(Input.GetAxis("D-PadLR") == 1 && dPadEnabled)         //move right
			{
				dPadEnabled = false;
				StartCoroutine(DPadInputCharacter(1, false));
			}
			else if(Input.GetAxis("D-PadLR") == -1 && dPadEnabled)   //move left
			{
				dPadEnabled = false;
				StartCoroutine(DPadInputCharacter(-1, false));
			}

			if(Input.GetAxis("D-PadUD") == 1 && dPadEnabled)         //move up
			{
				dPadEnabled = false;
				StartCoroutine(DPadInputCharacter(1, true));

			}
			else if(Input.GetAxis("D-PadUD") == -1 && dPadEnabled)   //move down
			{
				dPadEnabled = false;
				StartCoroutine(DPadInputCharacter(-1, true));

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

			characterButtons[buttonSelected].GetComponent<Button>().Select();
			//eventSystem.SetSelectedGameObject(characterButtons[buttonSelected]);

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

			case NetworkManager.e_NetworkMode.WAIT_RACERS:
				if(cart != null)
					cart.transform.Rotate (Vector3.up * spinSpeed * Time.deltaTime);
				break;

			case NetworkManager.e_NetworkMode.RACE:
				break;
		}
	

	}

	public void RefreshHostList()
	{
		clickSound.Play ();
		ClearServerButtons();
		NetworkManager.GetInstance().RefreshHostList();
	}

	public void ClearServerButtons()
	{
		foreach(GameObject go in serverButtons)
		{
			Destroy(go);
		}

		serverButtons.Clear ();
	}

	public void SwitchToJoin ()
	{
		confirmSound.Play ();
		SwapUI ("Join");
		buttonSelected = 0;
		current = e_UISubscreen.JOIN;
	}

	public void SwitchToCreate ()
	{
		confirmSound.Play ();
		SwapUI ("Create");
		selectPlayerNum = false;
		buttonSelected = 0;
		current = e_UISubscreen.CREATE;
	}

	public void CreateServer()
	{
		confirmSound.Play ();
		NetworkManager.GetInstance().CreateServer(trackButtons[buttonSelected].GetComponentInChildren<MapSelectUI>().actualSceneName, minPlayers-1);
		buttonSelected = 0;
		SwapUI("Select");
		NetworkManager.GetInstance().UpdateCartInfo(racers[0].GetComponent<PlayerData>().name);
	}
	
	public void JoinServer()
	{
		confirmSound.Play ();
		NetworkManager.GetInstance().JoinServer(buttonSelected, serverButtons[buttonSelected].GetComponent<GameServer>().mapName.text);
		buttonSelected = 0;
		SwapUI("Select");

		NetworkManager.GetInstance().UpdateCartInfo(racers[0].GetComponent<PlayerData>().name);
	}
	
	public void DecreasePlayers()
	{
		if(minPlayers > 1)
		{
			clickSound.Play ();
			minPlayers--;
		}
		else incorrectClick.Play();
	}

	public void IncreasePlayers()
	{
		clickSound.Play ();
		minPlayers++;
	}
	
	public void SelectPlayerNum()
	{
		trackButtons[buttonSelected].GetComponentInChildren<Button>().enabled = false;
		selectPlayerNum = true;
	}
	
	public void CreateServerButton(string serverName, string mapName, int currPlayers, int maxPlayers)
	{
		GameObject serverUIObject = (GameObject)Instantiate (serverUIPrefab, serverList.transform.position, serverList.transform.rotation);
		serverUIObject.transform.SetParent(serverList.transform);
		serverUIObject.GetComponent<GameServer>().SetUIData(serverName, mapName, currPlayers, maxPlayers);
		serverUIObject.GetComponent<RectTransform>().localScale = Vector3.one;

		serverButtons.Add (serverUIObject);

	}

	public void CreatePlayerTags(int playerCount)
	{
		for(int i = 0; i < playerCount; i++)
		{
			GameObject go = Instantiate(tagPrefab);
			go.transform.parent = tagGrid.transform;
			go.GetComponent<RectTransform>().localScale = Vector3.one;

			tags.Add (go.GetComponent<Tag>());
		}
	}

	public void UpdatePlayerTag(int tagNum, string charName, int playerNum)
	{
		//my tag
		if(tagNum == playerNum)
		{
			Debug.Log ("in my spot");
			return;
		}
		Debug.Log ("attempting to change to " + charName);

		if(tagNum > playerNum)
		{
			for(int i = 0; i < racers.Length; i++)
			{				
				if(racers[i].GetComponent<PlayerData>().name == charName)
				{
					PlayerData tempData = racers[i].GetComponent<PlayerData>();
					tags[tagNum-1].charSprite.sprite = tempData.characterSprite;
					tags[tagNum-1].characterName.text = tempData.name;
					break;
				}
			}
		}
		else
		{
			for(int i = 0; i < racers.Length; i++)
			{				
				if(racers[i].GetComponent<PlayerData>().name == charName)
				{
					PlayerData tempData = racers[i].GetComponent<PlayerData>();
					tags[tagNum].charSprite.sprite = tempData.characterSprite;
					tags[tagNum].characterName.text = tempData.name;
					break;
				}
			}
		}
	}

	public void SwitchMap(string actualSceneName)
	{
		clickSound.Play ();
		if(trackButtons[buttonSelected].GetComponentInChildren<Button>().enabled == false)
		{
			Debug.Log ("yup");
			trackButtons[buttonSelected].GetComponentInChildren<Button>().enabled = true;
			selectPlayerNum = false;
		}
		
		for(int i = 0; i < trackButtons.Count; i++)
		{
			if(trackButtons[i].GetComponentInChildren<MapSelectUI>().actualSceneName == actualSceneName)
				buttonSelected = i;
		}
	}

	public void SwitchKart(string name)
	{
		clickSound.Play ();
		for(int i = 0; i < racers.Length; i++)
		{
			ToggleBigHeadMode.BigHeadMode(bigHeadToggle.isOn);

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

				NetworkManager.GetInstance().UpdateCartInfo(characterButtons[buttonSelected].name);
			}
		}
	}
	
	public void SwapUI(string name)
	{
		switch(name)
		{
		case "Layout":
			layoutDisplay.SetActive(true);
			lobbyMenu.SetActive(false);
			joinMenu.SetActive(true);
			createMenu.SetActive(false);
			charSelectMenu.SetActive(false);
			break;
		case "Join":
			layoutDisplay.SetActive(false);
			lobbyMenu.SetActive(true);
			joinMenu.SetActive(true);
			createMenu.SetActive(false);
			charSelectMenu.SetActive(false);
			break;
		case "Create":
			lobbyMenu.SetActive(true);
			joinMenu.SetActive(false);
			createMenu.SetActive(true);
			charSelectMenu.SetActive(false);
			break;
		case "Select":
			lobbyMenu.SetActive(false);
			joinMenu.SetActive(false);
			createMenu.SetActive(false);
			charSelectMenu.SetActive(true);
			break;
		}
	}

	public IEnumerator ChangePlayerReq(int i)
	{
		minPlayers += i;
		if(minPlayers < 1)
		{
			minPlayers = 1;
			incorrectClick.Play ();
		}
		else clickSound.Play();
		
		yield return new WaitForSeconds(0.2f);
		dPadEnabled = true;
	}

	public IEnumerator DPadInputMaps(int value)
	{
		if(value == 1)  // go up
		{
			if(buttonSelected-1 >= 0)
			{
				buttonSelected -= 1;
				clickSound.Play();
			}
			else incorrectClick.Play();
		}
		else // go down
		{
			if(buttonSelected+1 < trackButtons.Count)
			{
				clickSound.Play();
				buttonSelected += 1;
			}
			else incorrectClick.Play();
		}


		if(buttonSelected < scrolledToPos)
		{
			scrolledToPos--;
			levelScrollView.localPosition  = new Vector3(-230f, levelScrollView.localPosition.y - levelScrollView.rect.height/trackButtons.Count, 0);
		}
		else if(buttonSelected >= scrolledToPos + 3)
		{
			scrolledToPos++;
			levelScrollView.localPosition  = new Vector3(-230f, levelScrollView.localPosition.y + levelScrollView.rect.height/trackButtons.Count, 0);
		}

		eventSystem.SetSelectedGameObject (trackButtons [buttonSelected]);		
		yield return new WaitForSeconds(0.3f);
		dPadEnabled = true;
	}

	public IEnumerator DPadInputServer(int value)
	{

		if(value == 1)  // go up
		{
			if(buttonSelected-1 < 0)
			{
				buttonSelected = serverButtons.Count-1;
				incorrectClick.Play();
			}
			else 
			{
				buttonSelected -= 1;
				clickSound.Play();
			}

		}
		else // go down
		{
			if(buttonSelected+1 >= serverButtons.Count)
			{
				buttonSelected = 0;
				incorrectClick.Play();
			}
			else 
			{
				buttonSelected += 1;
				clickSound.Play();
			}
		}

		
		yield return new WaitForSeconds(0.3f);
		dPadEnabled = true;
	}
		
	public IEnumerator DPadInputCharacter(int value, bool isVertical)
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

	public void SelectCharacter()
	{
		NetworkManager.GetInstance().setRacer(cart.GetComponent<PlayerData>().cartPrefab.gameObject);
	}

	public void StartRace()
	{
		AudioClip[] tempSelectSounds = cart.GetComponent<PlayerData>().charSelectSounds;

		if(tempSelectSounds.Length > 0)
		{
			int randomSound = Random.Range(0, tempSelectSounds.Length);

			charSelectSound.clip = tempSelectSounds[randomSound];
		}

		charSelectSound.Play ();

		charGrid.SetActive(false);
		dPadEnabled = false;
		waitingForPlayers.SetActive(true);

		if(Network.isServer)
			serverStartRace.gameObject.SetActive(true);
		else serverStartRace.gameObject.SetActive(false);

		SelectCharacter ();
	}
}
