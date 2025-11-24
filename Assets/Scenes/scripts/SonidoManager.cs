using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonidoManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sonidoBallBloque;
    public AudioClip sonidoBallPadell;

    // Update is called once per frame
    public void playBloque()
    {
        audioSource.PlayOneShot(sonidoBallBloque);
    }

    public void playPaddle()
    {
        audioSource.PlayOneShot(sonidoBallPadell);
    }
}
