using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleArmAnimation : MonoBehaviour {

    public ArmStateHandler currentArmState;
    private Animator animator;
    void Start() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        if (currentArmState.IsHold()) {
            animator.SetBool("hold", true);
        } else if (currentArmState.IsThrow()) {
            animator.SetBool("hold", false);
        }
    }

    public void ThrowAnimationIsFinished() {
        currentArmState.AlertThrowAnimationIsFinished();
    }
}
