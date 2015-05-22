using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapSelectUI : MonoBehaviour 
{
	public Image levelSprite = null;
	public Text levelName = null;

	public void SetUIData(TrackData trackdata)
	{
		levelSprite.sprite = trackdata.trackSprite;
		levelName.text = trackdata.displayTrackName;
	}
}
