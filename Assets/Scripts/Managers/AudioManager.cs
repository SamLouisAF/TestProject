using System;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioManager : MonoBehaviour
{


    [Header("------- Audio Clips -------")]

    public string areaTheme;
    public string[] areaAmbiance;


    public SoundSet musicSet;
    public SoundSet sfxSet;


    void Awake()
    {
      foreach(Sound s in musicSet.sounds)
        {
           
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.spatialBlend;

        }
        foreach (Sound s in sfxSet.sounds)
        {
           
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.spatialBlend;
        }
    }
    private void Start()
    {
/*        Play(areaTheme);
        foreach(string s in areaAmbiance)
        {
            Play(s);
        }*/
    }

    public void Play (string name)
    {
       Sound s = musicSet.sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            s = sfxSet.sounds.Find(sound => sound.name == name);
            if (s == null) return;
        }
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.spatialBlend = s.spatialBlend;
        s.source.Play();
        
    }

    public void Stop(String name)
    {
        Sound s = musicSet.sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            s = sfxSet.sounds.Find(sound => sound.name == name);
            if (s == null) return;
        }
        s.source.Stop();
    }
    public void PlayOneShot(String name)
    {
        Sound s = musicSet.sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            s = sfxSet.sounds.Find(sound => sound.name == name);
            if (s == null) return;
        }
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.spatialBlend = s.spatialBlend;
        s.source.PlayOneShot(s.clip);
    }

}
