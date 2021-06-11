using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] FxSounds;
    AudioSource audio;
    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void btnPlay()
    {
        audio.clip = FxSounds[0];
        audio.Play();
    }

    public void btnBack()
    {
        audio.clip = FxSounds[1];
        audio.Play();
    }

    public void inputFieldFX()
    {
        audio.clip = FxSounds[2];
        audio.Play();
    }

}
