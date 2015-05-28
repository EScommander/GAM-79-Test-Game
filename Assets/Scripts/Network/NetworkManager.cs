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

	public enum e_NetworkMode {SERVER_SELECT, CHARACTER_SELECT, MAP_SELECT, RACE};
	public e_NetworkMode current = e_NetworkMode.CHARACTER_SELECT;	

	public GameObject[] racers;
	private GameObject selectedPrefab = null;

	private string mapInfo = "";

	bool serversAdded = false;

	string levelToLoad = "city_scaled";

	int clientsReady = 0;

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
	}

	public void setRacer(GameObject cart)
	{
		selectedPrefab = cart;
		AddToReadyCount();

		if(Network.isServer)
			maxPlayers++;

		StartCoroutine(WaitForClients(0.1f));
	}

	public IEnumerator WaitForClients(float delay)
	{
		yield return new WaitForSeconds(delay);

		Debug.Log ("Clients ready?:" + clientsReady + "/" + maxPlayers);

		if(clientsReady == maxPlayers)
		{
			DontDestroyOnLoad (gameObject);
			Application.LoadLevel(levelToLoad);
			StartCoroutine(WaitForScene(1f));
		}
		else StartCoroutine(WaitForClients(0.1f));
	}

	public IEnumerator WaitForScene(float delay)
	{


		yield return new WaitForSeconds (delay);

		if(!Network.isServer)
			RequestCartInstantiate();
		else
		{
			StartCoroutine(ServerCartInstantiate());
		}

	}

	public void CreateServer(string levelToLoad, int maxPlayers)
	{
		this.maxPlayers = maxPlayers - 1;
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
			RefreshHostList();
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
		case e_NetworkMode.MAP_SELECT:
			break;
		case e_NetworkMode.RACE:
			GameObject countdownObj = GameObject.FindGameObjectWithTag("countdown");
			if(countdownObj != null)
			{
				countDownText = countdownObj.GetComponent<Text>();
			}




			//startPos = GameObject.FindGameObjectWithTag("StartPosition");
//			if (!connected && searchingForGame) 
//			{
//				if(this.randomizeTypeName)
//				{
//					StartServer();
//				}
//				else
//				{
//					RefreshHostList();
//					if(hostList != null)
//					{
//						Debug.Log (hostList.Length);
//						if(hostAttempt < hostList.Length)
//						{	
//							Network.Connect(hostList[hostAttempt]);
//							connected = true;
//						}
//						else StartServer();
//					}
//				}
//			}

			if(!countDownStarted && Network.isServer)
			{
				if(clients.Count >= maxPlayers)
				{
					SendRaceStartTime();
					Debug.Log ("START!");
					countDownStarted = true;
				}
			}

			if(isOnline)
			{
				if (!searchingForGame && !Network.isClient && !Network.isServer) 
				{
					searchingForGame = true;
				}
				
				
				
				if(!gameStarted && raceStart > Network.time)
				{
					int countDown = (int)(raceStart - Network.time) + 1;
					if(countDownText != null)
					{
						Debug.Log ("Countdownn?");
						countDownText.text = countDown.ToString();
					}
//					NetworkSyncedCart cartSync = myCart.gameObject.GetComponent<NetworkSyncedCart>();
//					if(cartSync != null)
//					{
//						cartSync.enabled = true;
//					}
//
//					NetworkView cartview = myCart.gameObject.GetComponent<NetworkView>();
//					if(cartview != null)
//					{
//						cartview.enabled = true;
//					}

					myCart.ActivateCart();
				}
				else if(!gameStarted && raceStart != -1)
				{
					gameStarted = true;

				}
				else if(gameStarted && raceStart + 2 > Network.time)
				{
					if(countDownText != null)
					{
						Debug.Log ("race!");
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
//		Debug.Log (Network.player);
//		Debug.Log ("whut?");
//		GameObject cartObject = (GameObject)Network.Instantiate(selectedPrefab, startPos.transform.position, selectedPrefab.transform.rotation, 0);
//		myCart = cartObject.GetComponent<CartController>();

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
		GameObject cartObject = (GameObject)Network.Instantiate(selectedPrefab, startPos.transform.position, selectedPrefab.transform.rotation, 0);
		myCart = cartObject.GetComponent<CartController>();
		current = e_NetworkMode.RACE;
	}

	[RPC]
	void RequestCartInstantiate()
	{
		GetComponent<NetworkView>().RPC ("ReceiveInstantiateRequest", RPCMode.Server);
	}

	[RPC]
	void ReceiveInstantiateRequest(NetworkMessageInfo info)
	{
		int racePos = clients.Count;
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
		
		GameObject cartObject= (GameObject)Network.Instantiate(selectedPrefab, startPos.transform.position + 
		                                                       new Vector3(3f * (pos%4), 0, selectedPrefab.transform.lossyScale.z * 5f * row), 
		                                                       selectedPrefab.transform.rotation, 0);
		myCart = cartObject.GetComponent<CartController>();
		current = e_NetworkMode.RACE;
	}

	void RefreshHostList()
	{
		MasterServer.RequestHostList (typeName);
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

	[RPC] void JoinClientListOnServer()
	{
		GetComponent<NetworkView>().RPC ("AddClientToList", RPCMode.Server, Network.player);
		//RequestCartInstantiate();
	}

	[RPC] void AddClientToList(NetworkPlayer player)
	{
		clients.Add (player);
	}


	[RPC] void SendRaceStartTime()
	{
		this.StartCountdownAudio ();
		float delay = 5.0f;
		raceStart = Network.time + delay;
		GetComponent<NetworkView>().RPC ("ReceiveRaceStartTime", RPCMode.OthersBuffered, delay);

	}

	[RPC] void ReceiveRaceStartTime(float delay)
	{
		this.StartCountdownAudio ();
		raceStart = Network.time + delay;
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
