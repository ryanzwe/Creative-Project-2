using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    private static SoundHandler instance;
    public static SoundHandler Instance
    {
        get { return instance; }
    }

    private AudioSource audio;
    public AudioClip[] SoundClips;

    public enum Sounds
    {
        Pickup_Gun = 0,
        Pickup_Extra = 1,
        Weather_Rain = 2,
        Weather_Thunder = 3,
        Objective_Complete = 4,
    }

    public Sounds curSound;
    private void Start()
    {
        instance = this;
        audio = GetComponent<AudioSource>();
    }

    public void PlaySound(Sounds sound)
    {
        audio.clip = SoundClips[(int) sound];
        audio.Play();
    }



}
