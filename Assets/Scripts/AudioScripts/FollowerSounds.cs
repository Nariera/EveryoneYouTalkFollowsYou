using UnityEngine;
using System.Collections;

public class FollowerSounds : MonoBehaviour {

    AudioSource audio;
    public AudioClip[] collideClips;
    public AudioClip startFollowClip;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    bool initialCollision = true;

	// Use this for initialization
	void Start () {
        audio = GetComponent<AudioSource>();
	}
	

    void OnCollisionEnter(Collision collider)
    {
        //So that nothing makes a sound on it's first collision.
        if (initialCollision)
        {
            initialCollision = false;
            return;
        }
        if (audio.isPlaying || collideClips == null || collideClips.Length == 0)
            return;


        int randomIndex = Random.Range(0, collideClips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        audio.pitch = randomPitch;
        audio.clip = collideClips[randomIndex];
        audio.Play();
    }
}
