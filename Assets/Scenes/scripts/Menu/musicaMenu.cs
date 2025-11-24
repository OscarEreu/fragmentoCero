using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicaMenu : MonoBehaviour
{
    private AudioSource music;
    public AudioClip ClickAudio;
    public AudioClip SecondAudio;
    
    void Start()
    {
        music = GetComponent<AudioSource>();
    }

    public void ClickAudioOn()
    {
        music.PlayOneShot(ClickAudio);
    }
    
    public void SecondAudioOn()
    {
         music.PlayOneShot(SecondAudio);
    }
}
