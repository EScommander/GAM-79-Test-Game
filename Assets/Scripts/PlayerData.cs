using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour 
{
	public Vector3 regularView = new Vector3(-7.75f, 2.2f, -4.45f);
	public Vector3 zoomedView = -Vector3.one;

	public Sprite characterSprite = null;
	public CartController cartPrefab = null;
	public string name = "";
}
