using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleSilverwareAudio : MonoBehaviour {
    public GameObject autoDestroyPlayerPrefab;
    public AudioClip[] fallClips;

    private void OnCollisionEnter(Collision collision) {
        if (collision.GetContact(0).normal == Vector3.up) {
            GameObject clone = Instantiate<GameObject>(autoDestroyPlayerPrefab, gameObject.transform.position, Quaternion.identity);
            clone.transform.parent = gameObject.transform;
            AudioSource audioSource = clone.GetComponent<AudioSource>();
            audioSource.clip = fallClips[Random.Range(0, fallClips.Length)];
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.volume = 1f;
            audioSource.Play();
        }
    }
}
