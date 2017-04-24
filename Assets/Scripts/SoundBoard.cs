using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBoard : MonoBehaviour {

    public static SoundBoard instance;

    public AudioClip Launch;
    public AudioClip Explosion;
    public AudioClip Gun;

    AudioSource source;

    void Start()
    {
        instance = this;
        source = GetComponent<AudioSource>();
    }

    public void PlayLaunch()
    {
        source.clip = Launch;
        source.Play();
    }

    public void PlayExplosion()
    {
        source.clip = Explosion;
        source.Play();
    }

    public void PlayGun()
    {
        source.clip = Gun;
        source.Play();
    }
}
