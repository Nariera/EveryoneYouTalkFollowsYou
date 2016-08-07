using UnityEngine;
using System.Collections;

public class FollowerSounds : MonoBehaviour {

    AudioSource audio;
    public AudioClip collideClip;
    public AudioClip startFollowClip;

    bool initialCollision = true;

	// Use this for initialization
	void Start () {
        audio = GetComponent<AudioSource>();
	}
	

    void OnCollisionEnter(Collision collider)
    {
        if (initialCollision)
        {
            initialCollision = false;
            return;
        }
        if (audio.isPlaying)
            return;
        audio.clip = collideClip;
        audio.Play();
        //audio.PlayOneShot(collideClip);
    }
}
