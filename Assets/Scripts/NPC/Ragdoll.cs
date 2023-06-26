using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour {
    public bool activateAtStart = false;

    public BodyPart[] bodyParts;
    private bool active;
    private bool isCollidedByTableware;

    private void Start() {
        SetActive(activateAtStart);
        isCollidedByTableware = false;
    }

    public bool IsActive() {
        return active;
    }

    public void SetActive(bool b) {
        foreach (BodyPart bp in bodyParts) {
            bp.rgbd.isKinematic = !b;
        }
        active = b;
    }

    void Update() {

        foreach (BodyPart bp in bodyParts) {
            isCollidedByTableware |= bp.IsCollidedByTableware();
        }

        if (isCollidedByTableware) {
            SetActive(true);
            isCollidedByTableware = false;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            // Debug.Log("Activate Rag doll = " + !active);
            SetActive(!active);
        }
    }
}
