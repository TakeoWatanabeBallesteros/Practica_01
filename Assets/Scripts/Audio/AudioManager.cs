using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup soundEffectsMixerGroup;

    public Sound[] sounds;
    public static AudioManager instance;

   
    public string ambient;
    public string music;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        //DontDestroyOnLoad(gameObject);
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

           switch(s.audioType)
            {
                case Sound.AudioTypes.soundEffect:
                    s.source.outputAudioMixerGroup = musicMixerGroup;
                    break;
                case Sound.AudioTypes.music:
                    s.source.outputAudioMixerGroup = soundEffectsMixerGroup;
                    break;
                default:
                    break;
            }
        }
    }
     void Start()
    {
        Play(ambient);
        Play(music);
    }

    public void Play(string name)
    {
       Sound s=  Array.Find(sounds, sound => sound.name == name);
        if (s==null)
        {
            Debug.LogWarning("Sound" + name + "Doesnt work!");
            
            return;
        }

       
        s.source.Play();
    }

    public void PlayBucle(string name)
    {
       Sound s=  Array.Find(sounds, sound => sound.name == name);
        if (s==null)
        {
            Debug.LogWarning("Sound" + name + "Doesnt work!");
            
            return;
        }

        s.source.loop = true;
        s.source.Play();
    }

    public void Stop(string name)
    {
       Sound s=  Array.Find(sounds, sound => sound.name == name);
        if (s==null)
        {
            Debug.LogWarning("Sound" + name + "Doesnt work!");
            
            return;
        }

       
        s.source.Stop();
    }

    public void UpdateMixerVolume()
    {
        musicMixerGroup.audioMixer.SetFloat("Music Volume", Mathf.Log10(AudioOptionsBehavior.musicVolume) * 20);
        soundEffectsMixerGroup.audioMixer.SetFloat("Sound EVolume", Mathf.Log10(AudioOptionsBehavior.soundEffectVolume) * 20);
    }

}
