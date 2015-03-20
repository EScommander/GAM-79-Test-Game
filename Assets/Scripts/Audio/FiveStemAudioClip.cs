using UnityEngine;
using System.Collections;

public class FiveStemAudioClip : MonoBehaviour 
{
	public AudioSource baseClip;
	public AudioSource[] stems;
	public float[] stemVolumes;

	private void Start () 
	{
		stemVolumes = new float[stems.Length];
		this.EqualizeAllStems ();
	}

	private void Update () 
	{
		for(int i = 0; i < stems.Length; i++)
		{
			stems[i].volume = this.stemVolumes[i];
		}
	}

	[ContextMenu("Mute All Stems")]
	public void MuteAllStems()
	{
		foreach(AudioSource stemSource in stems)
		{
			if(stemSource != null)
			{
				stemSource.mute = true;
			}
		}
	}

	[ContextMenu("Unmute All Stems")]
	public void UnMuteAllStems()
	{
		foreach(AudioSource stemSource in stems)
		{
			if(stemSource != null)
			{
				stemSource.mute = false;
			}
		}
	}

	[ContextMenu("Randomize All Stems")]
	public void RandomizeAllStems()
	{
		for(int i = 0 ; i < stemVolumes.Length; i++)
		{
			stemVolumes[i] = Random.Range(0.1f, 1.0f);
		}
	}

	[ContextMenu("Equalize All Stems")]
	public void EqualizeAllStems()
	{
		for(int i = 0 ; i < stemVolumes.Length; i++)
		{
			stemVolumes[i] = 1.0f;
		}
	}

	public void MuteStem(int index, bool mute)
	{
		if(this.stems.Length > index)
		{
			this.stems[index].mute = mute;
		}
	}
	
	public void MuteStem(AudioSource source, bool mute)
	{
		foreach(AudioSource stemSource in stems)
		{
			if(stemSource == source)
			{
				stemSource.mute = mute;
			}
		}
	}
	
	public void SetStemVolume(AudioSource source, float volume)
	{
		foreach(AudioSource stemSource in stems)
		{
			if(stemSource == source)
			{
				stemSource.volume = volume;
			}
		}
	}

	public void SetStemVolume(int index, float volume)
	{
		if(this.stems.Length > index)
		{
			this.stems[index].volume = volume;
		}
	}
}
