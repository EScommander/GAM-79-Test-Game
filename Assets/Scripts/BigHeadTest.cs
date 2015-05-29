using UnityEngine;
using System.Collections;

public class BigHeadTest : MonoBehaviour 
{
	private void Awake()
	{
		DontDestroyOnLoad (this.gameObject);
	}

	[ContextMenu("Enable Big Head Mode")]
	public void EnableBigHeadMode()
	{
		ToggleBigHeadMode.BigHeadMode (true);
	}

	[ContextMenu("Disable Big Head Mode")]
	public void DisableBigHeadMode()
	{
		ToggleBigHeadMode.BigHeadMode (false);
	}
}
