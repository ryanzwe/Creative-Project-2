using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class SoundHandler : MonoBehaviour
{
    private static SoundHandler instance;
    public static SoundHandler Instance
    {
        get { return instance; }
    }

    private AudioSource audio;
    public AudioClip[] SoundClips;
    public AudioClip[] BackgroundMusic;
    public AudioSource[] EffectPlayers;
    public AudioSource[] BackgroundPlayers;
    public float EffectsVol = 50;
    public float MusicVol = 50;
    public Slider MusicSlider;
    public Slider EffectsSlider;
    public Slider Sensitivityslider;
    public Text EffectsSliderText;
    public Text MusicSliderText;
    public Text SensitivityText;
    public enum Sounds
    {
        Pickup_Gun = 0,
        Pickup_Extra = 1,
        Weather_Rain = 2,
        Weather_Thunder = 3,
        Objective_Complete = 4,
    }
    private void Start()
    {
        instance = this;
        audio = GetComponent<AudioSource>();
    }
    private void Update()
    {
        EffectsSliderText.text = EffectsSlider.value.ToString("n0");
        MusicSliderText.text = MusicSlider.value.ToString("n0");
        SensitivityText.text = Sensitivityslider.value.ToString("n0");
    }

    public void PlaySound(Sounds sound)
    {
        audio.clip = SoundClips[(int)sound];
        audio.Play();
    }
    public void ApplySound()
    {
        EffectsVol = EffectsSlider.value;
        MusicVol = MusicSlider.value;
        ApplyVolume();
        PlayerPrefs.SetFloat("MouseSensitivity", Sensitivityslider.value);
        CharController.Instance.mouseSensitivity = Sensitivityslider.value;
        SoundSaver.Save(this);
    }
    public void InitSliders()
    {
        // Deserializing and loading the volumes 
        SoundSerialized volumes = SoundSaver.Load();
        if (volumes != null)
        {
            EffectsVol = volumes.EffectsVol;
            MusicVol = volumes.MusicVol;
        }
        
        // Setting the sliders volumes
        EffectsSlider.value = EffectsVol;
        MusicSlider.value = MusicVol;
        Sensitivityslider.value = PlayerPrefs.GetFloat("MouseSensitivity", 5);
    }
    private void ApplyVolume()
    {
        foreach (AudioSource aud in EffectPlayers)
        {
            aud.volume = EffectsVol / 100;
        }
        foreach (AudioSource aud in BackgroundPlayers)
        {
            aud.volume = MusicVol / 100;
        }
    }
}
public class SoundSaver
{
    static string FileName = "SoundSettings.Varyaty";
    public static void Save(SoundHandler lvls)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(Application.persistentDataPath + "/" + FileName, FileMode.Create);
        SoundSerialized ss = new SoundSerialized(lvls);
        bf.Serialize(fs, ss);
        fs.Close();

    }
    public static SoundSerialized Load()
    {
        if (File.Exists(Application.persistentDataPath + "/" + FileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(Application.persistentDataPath + "/" + FileName, FileMode.Open);
            SoundSerialized ss = (SoundSerialized)bf.Deserialize(fs);
            fs.Close();
            return ss;
        }
        else
        {
            Debug.Log("No Save Found!");
            return null;
        }

    }
}
[Serializable]
public class SoundSerialized
{
    public float EffectsVol = 50;
    public float MusicVol = 50;
    public SoundSerialized(SoundHandler lvls)
    {
        EffectsVol = lvls.EffectsVol;
        MusicVol = lvls.MusicVol;
    }
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

