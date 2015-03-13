using UnityEngine;
using System.Collections;

public class HologramController : MonoBehaviour 
{
	public bool mainController = false;
	public GameObject holoCart;
	public GameObject mainCart;

	public float Speed = 1.0f;

	public float animTime = 0.0f;
	public float delay = 0.5f;


	private Material hologramShader;

	private float delayTimer = 0.0f;

	// Use this for initialization
	void Start () 
	{
		if(!mainController)
		{
			hologramShader = this.renderer.material;
			if(hologramShader.HasProperty("_AnimTime"))
			{
				hologramShader.SetFloat ("_AnimTime", animTime);
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
			delayTimer += Time.deltaTime;

			if(delayTimer >= this.delay)
			{

				animTime += Time.deltaTime * this.Speed;

				if(!mainController && hologramShader.HasProperty("_AnimTime"))
				{
					hologramShader.SetFloat ("_AnimTime", animTime);
				}

				if(animTime >= 2.5f)
				{
					this.Speed *= -1;

					if(this.mainController && this.mainCart != null && this.holoCart != null)
					{
						this.holoCart.SetActive(false);
					}
				}
				else
				{
					if(this.mainController && this.mainCart != null && this.holoCart != null)
					{
						this.holoCart.SetActive(true);
					}
				}

				if(animTime >= 1.1f)
				{
					if(this.mainController && this.mainCart != null)
					{
						this.mainCart.SetActive(true);
					}
				}
				else if(animTime <= 0.5f)
				{
					if(this.mainController && this.mainCart != null)
					{
						this.mainCart.SetActive(false);
					}
				}

				if(animTime <= -1.5f)
				{
					this.Speed *= -1;
				}
			}
	}
}
