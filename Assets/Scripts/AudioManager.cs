﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mainMixer;
    public AudioSource audioSourceLoops;
    private AudioClip startLoop;
    private AudioSource audioSourcePauseMenu;
    private List<AudioSource> fxChannels = new List<AudioSource>();
    private int channelToUse = 0;


    void Start()
    {
        GameObject fxGO = transform.Find("FX").gameObject;
        for (int i = 0; i < 12; i++)
        {
            AudioSource newAS = fxGO.AddComponent<AudioSource>();
            fxChannels.Add(newAS);
        }
        audioSourceLoops = transform.Find("Loops").GetComponent<AudioSource>();
        audioSourcePauseMenu = transform.Find("Pause Menu").GetComponent<AudioSource>();
        startLoop = audioSourceLoops.clip;

    }

    public void PlaySound(string name, float customPitch=1)
    {
        channelToUse++;
        if(channelToUse >= fxChannels.Count) channelToUse=0;
        AudioClip ac;
        AudioSource audioSourceFX = fxChannels[channelToUse];
        audioSourceFX.volume = 1;
        audioSourceFX.pitch = 1;
        bool uniquePitch = true;

        if (name == "Get Coin")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/coin get");
            audioSourceFX.pitch = customPitch;
            uniquePitch = false;
        }
        else if (name == "Shoot Fish")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/hitmarker");
            audioSourceFX.volume = .3f;
        }
        else if (name == "Spawn Food")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/pop");
        }
        else if (name == "Spawn Fish")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/splash");
            audioSourceFX.volume = 1.5f;

        }
        else if (name == "Fish Ate")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/chew");
        }
        else if (name == "Enemy Fish Chomp")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/chew 2");
            audioSourceFX.volume = 1f;
        }
        else if (name == "Error")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/decline");
            audioSourceFX.volume = .7f;
            uniquePitch = false;
        }
        else if (name == "Combat Start")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/worm");
        }
        else if (name == "Combat Over")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/siz");
            audioSourceFX.volume = .5f;
            uniquePitch = false;
        }
        else if (name == "Buy Upgrade")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/wah");
            audioSourceFX.volume = .1f;
            uniquePitch = false;
        }
        else if (name == "Fish Death")
        {
            ac = GetRandomClip("Audio/FX/fish death");
        }
        else if (name == "Enemy Fish Death")
        {
            ac = GetRandomClip("Audio/FX/fish death");
            audioSourceFX.pitch = .7f;
        }
        else if (name == "Poke")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/bubble 1");
            audioSourceFX.volume = .4f;
            audioSourceFX.pitch = 3;
        }
        else
        {
            Debug.Log("hey that wasn't one of the audio options.");
            return;
        }

        if (uniquePitch) audioSourceFX.pitch += Random.Range(-0.2f, 0.2f);
        audioSourceFX.clip = ac;
        audioSourceFX.PlayOneShot(audioSourceFX.clip);
    }

    private AudioClip GetRandomClip(string path)
    {
        Object[] clips = Resources.LoadAll(path, typeof(AudioClip));
        return (AudioClip)clips[Random.Range(0,clips.Length)];
    }

    public void CombatTime()
    {
        StopAllCoroutines();
        audioSourceLoops.clip = (AudioClip)Resources.Load("Audio/Music/fish combat");
        audioSourceLoops.volume = .6f;
        audioSourceLoops.Play();
    }

    public void CombatOver()
    {
        audioSourceLoops.clip = startLoop;
        audioSourceLoops.volume = .75f;
        FadeAudioLoops(5, true);
        PlaySound("Combat Over");
    }

    public void PauseMenu(bool paused)
    {
        if(paused)
        {
            audioSourceLoops.Pause();
            audioSourcePauseMenu.Play();
        } else {
            audioSourceLoops.UnPause();
            audioSourcePauseMenu.Pause();
        }
    }

    /// <summary>
    /// set fadeIn to true to fade in, false to fade out
    /// </summary>
    public void FadeAudioLoops(float fadeTime, bool fadeIn)
    {
        Debug.Log("Fading audio loop for " + fadeTime + " seconds.");
        if (fadeIn)
        {
            this.StartCoroutine(FadeIn(audioSourceLoops, fadeTime));
        }
        else
        {
            this.StartCoroutine(FadeOut(audioSourceLoops, fadeTime));
        }

    }

    public IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    public IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;
        audioSource.volume = 0f;
        audioSource.Play();
        while (audioSource.volume < startVolume)
        {
            audioSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSource.volume = startVolume;
    }

    // set mixer volume
    public void SetLoopsMixerVolume(float percent)
    {
        // i bet you'll never figure out why * 20 is the correct value here
        this.mainMixer.SetFloat("LoopsVolume", Mathf.Log(percent) * 20);
    }
    public void SetFXMixerVolume(float percent)
    {
        this.mainMixer.SetFloat("FXVolume", Mathf.Log(percent) * 20);
    }
}
