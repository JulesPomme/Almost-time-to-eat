using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateArmState : MonoBehaviour {

    public ArmStateHandlerSO currentArmState;
    public BooleanSO isTablewareThrowable;

    private void Start() {
        currentArmState.SetToIdle();
    }

    void Update() {
        if (Input.GetMouseButton(0) && isTablewareThrowable.value) {
            currentArmState.SetToHold();
        } else if (Input.GetMouseButtonUp(0)) {
            currentArmState.SetToThrow();
        }

        if (currentArmState.IsThrow() && !currentArmState.IsThrowAnimationOn()) {
            currentArmState.SetToIdle();
        }
    }
}
