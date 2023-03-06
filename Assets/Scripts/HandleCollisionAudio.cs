using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCollisionAudio : MonoBehaviour {

    [System.Serializable]
    public struct SurfaceToAudioMap {
        public SurfaceSO[] surfaces;
        public AudioClip[] clips;
        public float volume;
    }

    public bool active = true;
    public GameObject autoDestroyPlayerPrefab;
    public SurfaceToAudioMap[] surfaceToAudiosMap;

    private void OnCollisionEnter(Collision collision) {
        if (!active)
            return;
        //Retrieving the surface of the colliding object...
        Surface surface = collision.gameObject.GetComponent<Surface>();
        //...if the surface exists and a map is assigned...
        if (surface != null && surfaceToAudiosMap != null) {
            //...do we have a bunch of sounds for this surface?...
            AudioClip[] clips = null;
            float volume = 1f;
            bool found = false;
            int i = 0;
            while (!found && i < surfaceToAudiosMap.Length) {
                int j = 0;
                while (!found && j < surfaceToAudiosMap[i].surfaces.Length) {
                    if (surfaceToAudiosMap[i].surfaces[j] == surface.surfaceSO) {
                        clips = surfaceToAudiosMap[i].clips;
                        volume = surfaceToAudiosMap[i].volume;
                        found = true;
                    }
                    j++;
                }
                i++;
            }
            //...if yes, play one of them!
            if (found) {
                GameObject clone = Instantiate<GameObject>(autoDestroyPlayerPrefab, gameObject.transform.position, Quaternion.identity);
                clone.transform.parent = gameObject.transform;
                AudioSource audioSource = clone.GetComponent<AudioSource>();
                audioSource.clip = clips[Random.Range(0, clips.Length)];
                audioSource.pitch = Random.Range(0.8f, 1.2f);
                audioSource.volume = volume;
                audioSource.Play();
            }
        }
    }
}
