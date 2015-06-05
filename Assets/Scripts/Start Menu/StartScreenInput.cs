using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartScreenInput : MonoBehaviour 
{
	//public Object jukebox = null;

	public Text start = null;
	public float fadeDur = 0.2f;
	float holdAtFull = 1.5f;

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(FadeAlpha(fadeDur)); 
		LoadFade blurp = LoadFade.SceneInstance;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.anyKey)
		{
			StopAllCoroutines();
			start.gameObject.SetActive(false);

			StartCoroutine(DelayedLoad());

		}
	}

	public IEnumerator DelayedLoad()
	{
		LoadFade.SceneInstance.FadeOut();

		yield return new WaitForSeconds(LoadFade.SceneInstance.fadeDur);

		Application.LoadLevel("Menu_CharacterSelect");
	}

	public IEnumerator FadeAlpha(float delay)
	{
		if(start.canvasRenderer.GetColor().a == 0)
		{
			start.CrossFadeAlpha(1, fadeDur, false);
			yield return new WaitForSeconds(delay);
			
			StartCoroutine(FadeAlpha(fadeDur));

		}
		else
		{
			start.CrossFadeAlpha(0, fadeDur, false);
			yield return new WaitForSeconds(delay);
			
			StartCoroutine(FadeAlpha(fadeDur + holdAtFull)); //holds for 2 seconds at full alpha
		}



	}
}
