﻿using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{

	public AudioSource efxSource;
	public AudioSource musicSource;
	public static SoundManager instance = null;

	public float lowPitchRange = 0.95f;
	public float highPitchRange = 1.05f;

	void Awake ()
	{
		if (instance == null)
		{
			//DontDestroyOnLoad (gameObject);
			instance = this;
		} else if (instance != this)
			Destroy (gameObject);


	}

	public void PlaySingle (AudioClip clip, float volume = 1)
	{
		efxSource.clip = clip;
		efxSource.volume = volume;
		efxSource.Play ();
	}

	public void RandomizeSfx (params AudioClip[] clips)
	{
		int randomIndex = Random.Range (0, clips.Length);
		float randomPitch = Random.Range (lowPitchRange, highPitchRange);

		efxSource.pitch = randomPitch;
		efxSource.clip = clips [randomIndex];
		efxSource.Play ();
	}


}