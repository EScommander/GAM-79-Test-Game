using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadFade : MonoBehaviour 
{
	public bool fadeOnLoad = false;
	private Image blackScreen = null;

	public float fadeDur = 1f;

	public static LoadFade SceneInstance
	{
		get
		{
			if(_Instance == null)
			{
				GameObject go = (GameObject)Instantiate(Resources.Load ("BlackScreen"));
				_Instance = go.GetComponent<LoadFade>();
				_Instance.transform.parent = GameObject.Find("Canvas").transform;

				RectTransform rekt = go.GetComponent<Image>().rectTransform;
				go.GetComponent<Image>().rectTransform.localScale = Vector3.one;
				go.GetComponent<Image>().rectTransform.offsetMin = new Vector2(0, 0);
				go.GetComponent<Image>().rectTransform.offsetMax = new Vector2(0, 0);

			}
			return _Instance;
		}
	}

	private static LoadFade _Instance;

	void Awake()
	{
		_Instance = this;
		blackScreen = gameObject.GetComponent<Image>();
	}

	// Use this for initialization
	void Start () 
	{
		if(fadeOnLoad)
		{
			blackScreen.canvasRenderer.SetAlpha(1f);
			if(blackScreen != null)
			{
				blackScreen.enabled = true;
				blackScreen.CrossFadeAlpha(0f, 1.5f, false);
			}
		}

		StartCoroutine(SetInactive(1.5f));
	}

	IEnumerator SetInactive(float delay)
	{
		yield return new WaitForSeconds(delay);

		gameObject.SetActive(false);
	}

	public void FadeOut()
	{
		gameObject.SetActive(true);
		blackScreen = gameObject.GetComponent<Image>();
		if(blackScreen != null)
		{
			blackScreen.canvasRenderer.SetAlpha(0f);
			if(!blackScreen.enabled)
				blackScreen.enabled = true;
			blackScreen.CrossFadeAlpha(1f, fadeDur, false);
		}
	}
}
