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

    public void PlaySound(string name, bool uniquePitch=true)
    {
        AudioClip ac;
        if(name == "Coin"){
            ac = (AudioClip)Resources.Load("Audio/FX/Coin Get");
        }
        else if(name == "Spawn Food")
        {
            ac = (AudioClip)Resources.Load("Audio/FX/Pop");
        }
        else{
            return;
        }
        if(uniquePitch) audioSourceFX.pitch = Random.Range(0.8f, 1.2f);
        else audioSourceFX.pitch = 1;
        audioSourceFX.clip = ac;
        audioSourceFX.PlayOneShot(audioSourceFX.clip);
    }
}
