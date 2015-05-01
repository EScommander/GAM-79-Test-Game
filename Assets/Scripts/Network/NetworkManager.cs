using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// Make a web build to test out on a single machine
// Web build should be the opposite of what you are working on.

public class NetworkManager : MonoBehaviour 
{
	private const string typeName = "Norco-Studio-Class";
	private string gameName = "KartRacer";

	private HostData[] hostList; // USED FOR CLIENT ONLY
	private Vector2 scrollPosition;
	private Vector2 scrollPosition2;

	private List<NetworkPlayer> clients = new List<NetworkPlayer>();

	private List<string> conversation = new List<string>();

	private static NetworkManager instance_;

	public GameObject carPrefab;

	public bool isOnline = false;

	public bool countDownStarted = false;
	public static bool gameStarted = false;

	public int minPlayers = 12;

	int row = 0;

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


	// Use this for initialization
	void Start () 
	{
		startPos = GameObject.FindGameObjectWithTag("StartPosition");

		conversation.Add ("Connected.");
		instance_ = this;

		if(!isOnline)
		{
			GameObject myCar = (GameObject)Instantiate(carPrefab, carPrefab.transform.position, carPrefab.transform.rotation);
		}




		//MasterServer.ipAddress = YOUR OWN SERVER IP
	}

	// Update is called once per frame
	void Update () 
	{
		if (!connected && searchingForGame) 
		{
			RefreshHostList();
			if(hostList != null)
			{
				if(hostAttempt < hostList.Length)
				{
					Network.Connect(hostList[hostAttempt]);
					connected = true;
				}
				else StartServer();
			}
		}

		if(!countDownStarted && Network.isServer)
		{
			if(clients.Count >= minPlayers)
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

				countDownText.text = countDown.ToString();
			}
			else if(!gameStarted && raceStart != -1)
			{
				gameStarted = true;
			}
			else if(raceStart< Network.time)
			{
				countDownText.text = "GO!";
			}
			else 
				countDownText.gameObject.SetActive(false);
		}
	}

	public static NetworkManager GetInstance()
	{
		if (instance_ == null) 
		{
			GameObject go = new GameObject("_NetworkManager");
			instance_ = (NetworkManager)go.AddComponent<NetworkManager>();
			go.AddComponent<NetworkView>();
		}

		return instance_;
	}

	void OnGUI()
	{

	}

	private void StartServer()
	{
							  //(int connections, int listenPort, bool useNat) -- 20 used in FTP, 80 used for Internet traffic
		Network.InitializeServer (minPlayers, 25000, !Network.HavePublicAddress ());
		MasterServer.RegisterHost (typeName, gameName);
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
		//SpawnPlayer ();
		//CharacterUI.GetInstance ().EnableGUI();
//		CharacterUI.GetInstance ().NetworkInstantiateCharacter ();
//		GameStateManager.GetInstance ().ConnectedToNetwork ();
		clients.Add (Network.player);
		connected = true;
		Debug.Log (Network.player);
		Debug.Log ("whut?");
		GameObject myCar = (GameObject)Network.Instantiate(carPrefab, startPos.transform.position, carPrefab.transform.rotation, 0);

		Debug.Log ("Server Initialized");
	}

	void OnFailedToConnect(NetworkConnectionError error)
	{
		hostAttempt++;
		connected = false;
	}

	void OnConnectedToServer()
	{
		Debug.Log ("Joined Server!");
		connected = true;
		JoinClientListOnServer ();
		int playerPos = int.Parse(Network.player.ToString());
		Debug.Log (playerPos);
		int row = playerPos/4;

		GameObject myCar = (GameObject)Network.Instantiate(carPrefab, startPos.transform.position + new Vector3(carPrefab.transform.localScale.x * 1.5f, 0, carPrefab.transform.localScale.z * 1.5f * row), carPrefab.transform.rotation, 0);

		//CharacterUI.GetInstance ().NetworkInstantiateCharacter ();
		//GameStateManager.GetInstance ().ConnectedToNetwork ();
		//SpawnPlayer();
	}

	void GetHostList()
	{

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

	[RPC] void JoinClientListOnServer()
	{
		GetComponent<NetworkView>().RPC ("AddClientToList", RPCMode.Server, Network.player);
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
		GetComponent<NetworkView>().RPC ("ReceiveRaceStartTime", RPCMode.Others, delay);

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
