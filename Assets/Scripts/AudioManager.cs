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

    public void PlaySound(string name)
    {
        AudioClip ac;
        if(name == "Coin"){
            ac = (AudioClip)Resources.Load("Audio/Coin Get");
        }
        else{
            return;
        }
        audioSourceFX.clip = ac;
        audioSourceFX.PlayOneShot(audioSourceFX.clip);
    }
}
