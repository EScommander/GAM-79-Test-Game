using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour 
{
	private const string typeName = "Norco-Studio2";
	private string gameName = "Race";

	private HostData[] hostList; // USED FOR CLIENT ONLY
	private Vector2 scrollPosition;
	private Vector2 scrollPosition2;

	private List<NetworkPlayer> clients = new List<NetworkPlayer>();

	private static NetworkManager instance_;

	public GameObject carPrefab;

	public bool isOnline = false;

	public bool randomizeGameName = false;

	public bool countDownStarted = false;
	public static bool gameStarted = false;

	public CartController myCart = null;

	public int maxPlayers = 12;

	double raceStart = -1;

	public GUIStyle countDownStyle;

	bool searchingForGame = false;
	bool connected = false;
	int hostAttempt = 0;

	public GameObject startPos = null;

	public AudioSource countdownSource;
	public AudioClip clip;
//	private CharacterUI charUI;

	public Text countDownText = null;

	public enum e_NetworkMode {SERVER_SELECT, CHARACTER_SELECT, WAIT_RACERS, RACE};
	public e_NetworkMode current = e_NetworkMode.CHARACTER_SELECT;	

	public GameObject[] racers;
	private GameObject selectedPrefab = null;

	private string mapInfo = "";

	bool serversAdded = false;

	string levelToLoad = "city_scaled";

	int clientsReady = 0;
	int clientsLoaded = 0;

	bool bigHeadModeEnabled = false;

	private int myRacePos = 0;
	private bool overrideRaceStart = false;

	// Use this for initialization
	void Awake () 
	{
		Object[] objects = Resources.LoadAll("Carts");
		racers = new GameObject[objects.Length];
		for(int i = 0; i<objects.Length; i++)
		{
			racers[i] = (GameObject)objects[i];
			Debug.Log (racers[i].name);
		}

		instance_ = this;

		if(!isOnline)
		{
			GameObject myCar = (GameObject)Instantiate(carPrefab, carPrefab.transform.position, carPrefab.transform.rotation);
		}

		RefreshHostList();
	}

	public void setRacer(GameObject cart)
	{
		bigHeadModeEnabled = UIManager.GetInstance ().bigHeadToggle.isOn;
		selectedPrefab = cart;

		AddToReadyCount();
		current = e_NetworkMode.WAIT_RACERS;

	}

//	public IEnumerator WaitForClients(float delay)
//	{
//		yield return new WaitForSeconds(delay);
//
//		Debug.Log ("Clients ready?:" + clientsReady + "/" + maxPlayers);
//
//		if(clientsReady == maxPlayers)
//		{
//			DontDestroyOnLoad (gameObject);
//			Application.LoadLevel(levelToLoad);
//			StartCoroutine(WaitForScene(1f));
//		}
//		else StartCoroutine(WaitForClients(0.1f));
//	}

	public void LoadRace()
	{
		LoadFade.SceneInstance.FadeOut();
		StartCoroutine(LoadRaceAfterFade());
	}

	IEnumerator LoadRaceAfterFade()
	{
		yield return new WaitForSeconds(LoadFade.SceneInstance.fadeDur);

		// There is no reason to send any more data over the network on the default channel,
		// because we are about to load the level, thus all those objects will get deleted anyway
		Network.SetSendingEnabled(0, false);	
		
		// We need to stop receiving because first the level must be loaded.
		// Once the level is loaded, RPC's and other state update attached to objects in the level are allowed to fire
		Network.isMessageQueueRunning = false;
		
		DontDestroyOnLoad (gameObject);
		Application.LoadLevel(levelToLoad);
		StartCoroutine(WaitForScene(1f));
	}

	public IEnumerator WaitForScene(float delay)
	{
		yield return new WaitForSeconds (delay);

		// Allow receiving data again
		Network.isMessageQueueRunning = true;
		// Now the level has been loaded and we can start sending out data
		Network.SetSendingEnabled(0, true);

		if(!Network.isServer)
		{
			RequestCartInstantiate();
		}
		else
		{
			StartCoroutine(ServerCartInstantiate());
		}

		current = e_NetworkMode.RACE;

	}

	public void StartOverride()
	{
		Debug.Log ("step one");
		StopAllCoroutines();
		overrideRaceStart = true;
	}

	public void CreateServer(string levelToLoad, int maxPlayers)
	{
		this.maxPlayers = maxPlayers;
		this.levelToLoad = levelToLoad;
		this.mapInfo = levelToLoad;
		StartServer();
		current = e_NetworkMode.CHARACTER_SELECT;
	}
	
	public void JoinServer(int i, string levelToLoad)
	{
		this.levelToLoad = levelToLoad;
		Network.Connect(hostList[i]);
		maxPlayers = hostList[i].playerLimit;
		current = e_NetworkMode.CHARACTER_SELECT;
	}

	// Update is called once per frame
	void Update () 
	{

		switch(current)
		{
		case e_NetworkMode.SERVER_SELECT:
			if(hostList != null && !serversAdded)
			{
				for(int i = 0; i< hostList.Length; i++)
				{
					UIManager.GetInstance().CreateServerButton(hostList[i].gameName, hostList[i].comment, hostList[i].connectedPlayers, hostList[i].playerLimit);
				}
				serversAdded = true;
			}
			break;
		case e_NetworkMode.CHARACTER_SELECT:
			break;
		case e_NetworkMode.WAIT_RACERS:
			if(!countDownStarted && Network.isServer)
			{ 
				if(clientsReady >= maxPlayers || overrideRaceStart)
				{
					maxPlayers = clients.Count;
					clientsReady = 0;
					SendLoadLevel();
					Debug.Log ("START!");
					countDownStarted = true;
				}
			}
			break;

		case e_NetworkMode.RACE:
			GameObject countdownObj = GameObject.FindGameObjectWithTag("countdown");
			if(countdownObj != null)
			{
				countDownText = countdownObj.GetComponent<Text>();
			}

			if(isOnline)
			{
				if (!searchingForGame && !Network.isClient && !Network.isServer) 
				{
					searchingForGame = true;
				}
				
				
				
				if(!gameStarted && raceStart > Network.time)
				{
					myCart.ActivateCart();
					if(bigHeadModeEnabled)
						ToggleBigHeadMode.BigHeadMode(true);

					int countDown = (int)(raceStart - Network.time) + 1;
					if(countDownText != null)
					{
						countDownText.text = countDown.ToString();
					}

				}
				else if(!gameStarted && raceStart != -1)
				{
					gameStarted = true;
				}
				else if(gameStarted && raceStart + 2 > Network.time)
				{
					if(countDownText != null)
					{

						countDownText.text = "GO!";
					}
				}
				else if(gameStarted && raceStart + 2 < Network.time)
				{
					if(countDownText != null)
					{
						countDownText.gameObject.SetActive(false);
					}
				}
			}
			break;
		}
	}
	

	public static NetworkManager GetInstance()
	{
		if (instance_ == null) 
		{
			GameObject go = new GameObject("_NetworkManager");
			instance_ = (NetworkManager)go.AddComponent<NetworkManager>();
		}

		return instance_;
	}

	private void StartServer()
	{
							  //(int connections, int listenPort, bool useNat) -- 20 used in FTP, 80 used for Internet traffic
		Network.InitializeServer (maxPlayers, 25000, !Network.HavePublicAddress ());

		if(this.randomizeGameName)
		{
			MasterServer.RegisterHost(typeName, (gameName+ (Mathf.FloorToInt(Random.value * 100)) + ""), mapInfo);
		}
		else
		{
			MasterServer.RegisterHost(typeName, gameName, mapInfo);
		}
	}

	// LOWER METHODS ONLY USED FOR CLIENT ONLY!!
	private void JoinServer(HostData hostData)
	{
		Network.Connect (hostData);
	}

	public void LeaveServer()
	{
		if(Network.isServer)
		{
			Network.Disconnect();
			MasterServer.UnregisterHost();
		}
		else
		{
			Network.CloseConnection(Network.player, true);
		}
	}

	void OnNetworkInstantiate(NetworkMessageInfo info)
	{

	}

	void OnServerInitialized()
	{
		if (selectedPrefab == null)
			selectedPrefab = carPrefab;

		clients.Add (Network.player);
		connected = true;

		Debug.Log ("Server Initialized");
	}

	void OnFailedToConnect(NetworkConnectionError error)
	{
		hostAttempt++;
		connected = false;
	}

	void OnConnectedToServer()
	{
		Debug.Log ("connected?");
		if (selectedPrefab == null)
			selectedPrefab = carPrefab;

		Debug.Log ("Joined Server!");
		connected = true;
		JoinClientListOnServer ();


	}

	IEnumerator ServerCartInstantiate()
	{
		while(startPos == null)
		{
			startPos = GameObject.FindGameObjectWithTag("StartPosition");
		}

		yield return new WaitForSeconds(0f);

		GameObject cartObject = (GameObject)Network.Instantiate(selectedPrefab, startPos.transform.position, startPos.transform.rotation, 0);
		myCart = cartObject.GetComponent<CartController>();

		AddToLoadedCount();
	}

	[RPC]
	void RequestCartInstantiate()
	{
		GetComponent<NetworkView>().RPC ("ReceiveInstantiateRequest", RPCMode.Server);
	}

	[RPC]
	void ReceiveInstantiateRequest(NetworkMessageInfo info)
	{
		//int racePos = clients.Count;

		int racePos = 0;
		for(int i = 0; i < clients.Count; i++)
		{
			if(info.sender.ToString() == clients[i].ToString())
			{
				racePos = i;
				break;
			}
		}

		GetComponent<NetworkView>().RPC ("InstantiateBasedOnClientPos", RPCMode.AllBuffered, info.sender, racePos);
		current = e_NetworkMode.RACE;
	}

	[RPC]
	void InstantiateBasedOnClientPos(NetworkPlayer player , int pos)
	{
		if(player != Network.player)
			return;

		while(startPos == null)
		{
			startPos = GameObject.FindGameObjectWithTag("StartPosition");
		}
		
		Debug.Log ("my pos is:" + pos);
		int row = (pos-1)/4;
		
		GameObject cartObject= (GameObject)Network.Instantiate(selectedPrefab, startPos.transform.position + 3f * startPos.transform.right * (pos%4) - 
		                                                       startPos.transform.forward * selectedPrefab.transform.lossyScale.z * 5f * row, 
		                                                       startPos.transform.rotation, 0);
		myCart = cartObject.GetComponent<CartController>();

		AddToLoadedCount();
	}

	public void RefreshHostList()
	{
		Debug.Log ("yup");
		MasterServer.RequestHostList (typeName);

		serversAdded = false;
	}

	void OnMasterServerEvent(MasterServerEvent mse)
	{
		if (mse == MasterServerEvent.HostListReceived) 
		{
			hostList = MasterServer.PollHostList();
		}
	}

	[RPC] void AddToReadyCount()
	{
		GetComponent<NetworkView>().RPC ("ClientReady", RPCMode.AllBuffered);
	}

	[RPC] void ClientReady()
	{
		clientsReady ++;
	}

	[RPC] void AddToLoadedCount()
	{
		GetComponent<NetworkView>().RPC ("ClientLoaded", RPCMode.AllBuffered);
	}
	
	[RPC] void ClientLoaded()
	{
		clientsLoaded ++;
		
		if(clientsLoaded == maxPlayers)
			SendRaceStartTime();
	}

	[RPC] void JoinClientListOnServer()
	{
		GetComponent<NetworkView>().RPC ("AddClientToList", RPCMode.Server, Network.player);
	}

	[RPC] void AddClientToList(NetworkPlayer player)
	{
		clients.Add (player);
	}

	[RPC] void SendLoadLevel()
	{
		GetComponent<NetworkView>().RPC ("ReceivedLoadRequest", RPCMode.AllBuffered);
	}

	[RPC] void ReceivedLoadRequest()
	{
		LoadRace();
	}


	[RPC] void SendRaceStartTime()
	{
		this.StartCountdownAudio ();
		float delay = 5.0f;
		GetComponent<NetworkView>().RPC ("ReceiveRaceStartTime", RPCMode.AllBuffered, delay);

	}

	[RPC] void ReceiveRaceStartTime(float delay)
	{
		raceStart = Network.time + delay;
		current = e_NetworkMode.RACE;
	}
	
	private void StartCountdownAudio()
	{
		if(this.countdownSource != null)
		{
			this.countdownSource.Play();
		}
	}

//	// [RPC] = Remote Procedure Call needed for sending across network
//	[RPC] public void SendCharacterXMLToServer(string s, NetworkViewID networkViewID)
//	{
//
//		networkView.RPC ("ReceiveCharacterXMLFromClient", RPCMode.OthersBuffered, s, networkViewID);
//
//	}
//
//	// SERVER ONLY
//	[RPC] void ReceiveCharacterXMLFromClient(string charXML, NetworkViewID id)
//	{
//
//		GameObject[] avatars = GameObject.FindGameObjectsWithTag("Avatar");
//
//		for(int i = 0; i < avatars.Length; i++)
//		{
//			NetworkViewID idCheck = avatars[i].networkView.viewID;
//			Debug.Log(idCheck);
//
//			if(idCheck == id)
//			{
//				Debug.Log("Got here!");
//				CharacterUI.GetInstance().BuildCharacter (charXML, avatars[i]);
//			}
//		}
//	}

}
