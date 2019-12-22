using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSourceFX;

    void Start()
    {
        audioSourceFX = GetComponent<AudioSource>();
    }

    public void PlaySound(string name, bool uniquePitch=true, float basePitch=1f, float baseVolume=1f)
    {
        AudioClip ac;
        audioSourceFX.volume = baseVolume;
        audioSourceFX.pitch = basePitch;
        if(uniquePitch) audioSourceFX.pitch += Random.Range(-0.2f, 0.2f);

        if(name == "Coin"){
            ac = (AudioClip)Resources.Load("Audio/FX/coin get");
            audioSourceFX.volume = 1f;
        }
        else if(name == "Spawn Food")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/pop");
        }
        else if(name == "Spawn Fish")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/splash");
        }
        else if(name == "Fish Ate")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/chew");
        }
        else{
            return;
        }
        
        audioSourceFX.clip = ac;
        audioSourceFX.PlayOneShot(audioSourceFX.clip);
    }
}
