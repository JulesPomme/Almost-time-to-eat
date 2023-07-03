using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour {

    public bool activateAtStart = false;
    public BodyPart[] bodyParts;
    public float impactForce = 5;

    private bool active;
    private bool isCollidedByTableware;
    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
        SetActive(activateAtStart);
        isCollidedByTableware = false;
    }

    public bool IsActive() {
        return active;
    }

    public void SetActive(bool b) {
        animator.enabled = !b;
        foreach (BodyPart bp in bodyParts) {
            bp.rgbd.isKinematic = !b;
        }
        active = b;
    }

    void Update() {
        int i = 0;
        while (!isCollidedByTableware && i < bodyParts.Length) {
            isCollidedByTableware |= bodyParts[i].IsCollidedByTableware();
            i++;
        }

        if (isCollidedByTableware) {
            SetActive(true);
            isCollidedByTableware = false;
            bodyParts[i - 1].rgbd.AddForce(bodyParts[i - 1].GetImpactDirection() * impactForce, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            // Debug.Log("Activate Rag doll = " + !active);
            SetActive(!active);
        }
    }
}
