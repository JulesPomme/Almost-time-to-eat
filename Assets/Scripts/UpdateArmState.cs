using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateArmState : MonoBehaviour {

    public ArmStateHandler currentArmState;

    private void Start() {
        currentArmState.SetToIdle();
    }

    void Update() {
        if (Input.GetMouseButton(0)) {
            currentArmState.SetToHold();
        } else if (Input.GetMouseButtonUp(0)) {
            currentArmState.SetToThrow();
        }

        if (currentArmState.IsThrow() && !currentArmState.IsThrowAnimationOn()) {
            currentArmState.SetToIdle();
        }
    }
}
