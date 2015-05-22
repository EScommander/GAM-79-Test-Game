using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameServer : MonoBehaviour 
{
	public Text serverName = null;
	public Text mapName = null;
	public Text playerCount = null;

	public Image mapSprite = null;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void SetUIData(string serverName, string mapName, int currPlayers, int maxPlayers)
	{
		this.serverName.text = serverName;
		this.mapName.text = mapName;

		playerCount.text = currPlayers + "/" + maxPlayers;
	}
}
