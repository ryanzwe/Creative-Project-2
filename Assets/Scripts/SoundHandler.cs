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
/*

    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioSource sfxSource;

    Dictionary<string, AudioClip> AudioClips = new Dictionary<string, AudioClip>();

    // Use this for initialization
    void Start()
    {


        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio") as AudioClip[];
        foreach (AudioClip clip in clips)
        {
            AudioClips.Add(clip.name, clip);
        }

        LoadVolume();


    }

    public void PlayMusic(string name)
    {
        musicSource.clip = AudioClips[name];
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySingleSfx(string name)
    {
        sfxSource.Stop();
        sfxSource.PlayOneShot(AudioClips[name]);
    }

    public void LoadVolume()
    {
        // Load audio SFX and Music settings, sets audio sources
        musicSource.volume = PlayerPrefs.GetFloat("Music", 0.4f);
        sfxSource.volume = PlayerPrefs.GetFloat("SFX", 0.8f);

        for (int i = 0; i < sources.Count; i++)
        {
            sources[i].GetComponent<AudioSource>().volume = sfxSource.volume;
        }

    }

}
*/


}
