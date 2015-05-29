using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapSelectUI : MonoBehaviour 
{
	public Image levelSprite = null;
	public Text levelName = null;
	public string actualSceneName = "city_scaled";

	public void SetUIData(TrackData trackdata)
	{
		levelSprite.sprite = trackdata.trackSprite;
		levelName.text = trackdata.displayTrackName;
		this.actualSceneName = trackdata.actualSceneName; 
	}

	public void SwitchMap()
	{
		UIManager.GetInstance().SwitchMap(actualSceneName);
	}
}
