using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetAudioVolume : MonoBehaviour
{

    public AudioMixer mixer;

    public void SetVolume(float value) {

        mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    }
}
