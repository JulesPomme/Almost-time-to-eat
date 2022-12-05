using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeclareAudioSourceSO : MonoBehaviour
{
    public AudioSourceSO source;
    private void Awake() {
        source.source = GetComponent<AudioSource>();
    }
}
