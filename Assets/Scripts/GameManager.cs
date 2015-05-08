using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	public static GameManager SceneInstance
	{
		get
		{
			if(GameManager._instance == null)
			{
				GameObject go = new GameObject("GameManager");
				GameManager._instance = go.AddComponent<GameManager>();
				GameManager._instance.Init();
			}

			return GameManager._instance;
		}
	}

	public List<CartController> activeCarts;

	private static GameManager _instance;
	private bool initialized = false;

	private void Start()
	{
		if(!this.initialized)
		{
			this.Init ();
		}
	}

	private void Init()
	{
		initialized = true;

		if(GameManager.SceneInstance == null)
		{
			GameManager._instance = this;
		}
		else
		{
			Destroy(gameObject);
			Destroy(this);
		}

		activeCarts = new List<CartController> ();
	}
}
