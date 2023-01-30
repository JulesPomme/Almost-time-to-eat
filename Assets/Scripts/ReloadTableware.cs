using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadTableware : MonoBehaviour {

    public ScriptableObjectListSO availableTablewares;
    public AudioSource reloadSound;

    private bool entered;

    private void Start() {
        entered = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            entered = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            entered = false;
        }
    }

    private void Update() {
        if (entered && Input.GetMouseButtonDown(1)) {
            for (int i = 0; i < availableTablewares.list.Count; i++) {
                ((TablewareSO)availableTablewares.list[i]).ammo = ((TablewareSO)availableTablewares.list[i]).initAmmo;
                reloadSound.pitch = Random.Range(0.8f, 1.2f);
                reloadSound.Play();
            }
        }
    }
}
