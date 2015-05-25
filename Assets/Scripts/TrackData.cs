using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class TrackData
{
	public string displayTrackName = "";
	public Sprite trackSprite = null;
	public string actualSceneName = "";

	public enum e_TrackTypes {RACE, BATTLE};
	public e_TrackTypes type = e_TrackTypes.RACE;
}
