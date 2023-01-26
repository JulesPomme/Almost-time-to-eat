using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmStateController : MonoBehaviour {

    public ArmStateModelSO currentModel;
    public BooleanSO canThrow;


    private void Start() {
        currentModel.TellControllerIsStarted();
        currentModel.SetToIdle();
    }

    void Update() {

        currentModel.UpdatePreviousArmState();

        if (canThrow.yes) {
            if (Input.GetMouseButton(0)) {
                currentModel.SetToHold();
            } else if (Input.GetMouseButtonUp(0)) {
                currentModel.SetToThrow();
            }
        }

        if (canThrow.no || (currentModel.IsThrow() && !currentModel.IsThrowAnimationOn())) {
            currentModel.SetToIdle();
        }
    }
}
