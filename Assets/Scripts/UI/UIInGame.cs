using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIInGame : MonoBehaviour 
{
	public Text countdownText = null;
	public Text racePostion = null;
	public Text lapCount = null;
	public Image powerUp = null;
	public GameObject powerUpContainer = null;

	public Sprite[] powerUps = new Sprite[3];

	public static UIInGame SceneInstance
	{
		get
		{
			if(UIInGame._Instance == null)
			{
				GameObject go = new GameObject();
				go.name = "UI-InGame";
				UIInGame ui = (UIInGame)go.AddComponent<UIInGame>();
				UIInGame._Instance = ui;

				Debug.LogWarning("UI not found in scene!");
			}
			return UIInGame._Instance;
		}
	}

	[HideInInspector]
	private static UIInGame _Instance = null;


	public void SetPowerUpIcon(string name)
	{
		if(powerUp != null)
		{
			if(name == "GATLINGGUN")
			{
				powerUp.sprite = powerUps[0];
				powerUpContainer.SetActive(true);
			}
			else if(name == "ELECTRICTRAP")
			{
				powerUp.sprite = powerUps[1];
				powerUpContainer.SetActive(true);
			}
			else if(name == "SHIELD")
			{
				powerUp.sprite = powerUps[2];
				powerUpContainer.SetActive(true);
			}
			else
			{
				Debug.LogWarning("Power up not recognized in UI");
				powerUpContainer.SetActive(false);
			}
		}

	}

	public void PowerUpUsed()
	{
		if(powerUpContainer != null)
			powerUpContainer.SetActive(false);
	}

	void Awake()
	{
		if(_Instance == null)
		{
			_Instance = this;
		}
	}

	// Use this for initialization
	void Start () 
	{
		if(powerUpContainer != null)
			powerUpContainer.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
