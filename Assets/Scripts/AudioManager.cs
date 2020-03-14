using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mainMixer;
    private AudioSource audioSourceFX;
    private AudioSource audioSourceLoops;
    private AudioClip startLoop;

    void Start()
    {
        audioSourceFX = transform.Find("FX").GetComponent<AudioSource>();
        audioSourceLoops = transform.Find("Loops").GetComponent<AudioSource>();
        startLoop = audioSourceLoops.clip;

    }

    public void PlaySound(string name, bool uniquePitch = true, float basePitch = 1f, float baseVolume = 1f)
    {
        AudioClip ac;
        audioSourceFX.volume = baseVolume;
        audioSourceFX.pitch = basePitch;

        if (name == "Coin")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/coin get");
        }
        else if (name == "Spawn Food")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/pop");
        }
        else if (name == "Spawn Fish")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/splash");
        }
        else if (name == "Fish Ate")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/chew");
        }
        else if (name == "Error")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/decline");
        }
        else if (name == "Combat Start")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/worm");
        }
        else if (name == "Combat Over")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/siz");
        }
        else if (name == "Buy Upgrade")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/wah");
            audioSourceFX.volume = .7f;
            uniquePitch = false;
        }
        else if (name == "Fish Death")
        {
            ac = GetRandomClip("Audio/FX/fish death");
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
        audioSourceLoops.volume = 1;
        audioSourceLoops.Play();
        //PlaySound("Combat Start", false);

    }

    public void CombatOver()
    {
        audioSourceLoops.clip = startLoop;
        FadeAudioLoops(5, true);
        PlaySound("Combat Over", false, 1f, .5f);
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
