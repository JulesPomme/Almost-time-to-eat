using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ArmStateDescription")]
public class ArmStateModelSO : ScriptableObject {

    public ArmStateSO idleState;
    public ArmStateSO holdState;
    public ArmStateSO throwState;
    private ArmStateSO currentArmState;
    private ArmStateSO previousArmState;

    private bool isControllerStarted;
    private bool throwAnimationIsOn;

    public void Init() {
        previousArmState = null;
        isControllerStarted = false;
        throwAnimationIsOn = false;
    }

    public void SetToIdle() {
        currentArmState = idleState;
        throwAnimationIsOn = false;
    }

    public void SetToHold() {
        currentArmState = holdState;
        throwAnimationIsOn = false;
    }

    public void SetToThrow() {
        currentArmState = throwState;
        throwAnimationIsOn = true;
    }

    public bool IsIdle() {
        return currentArmState == idleState;
    }

    public bool IsThrow() {
        return currentArmState == throwState;
    }

    public bool IsHold() {
        return currentArmState == holdState;
    }

    public void AlertThrowAnimationIsFinished() {
        throwAnimationIsOn = false;
    }

    public bool IsThrowAnimationOn() {
        return throwAnimationIsOn;
    }

    public void PrintCurrentState() {
        if (currentArmState == idleState) {
            Debug.Log("### Arm state is idle");
        } else if (currentArmState == holdState) {
            Debug.Log("### Arm state is hold");
        } else if (currentArmState == throwState) {
            Debug.Log("### Arm state is throw");
        } else {
            Debug.Log("### Arm state is unknown");
        }
    }

    public void UpdatePreviousArmState() {
        previousArmState = currentArmState;
    }

    public bool StateHasChanged() {
        return previousArmState != currentArmState;
    }

    public void TellControllerIsStarted() {
        isControllerStarted = true;
    }

    public bool IsControllerStarted() {
        return isControllerStarted;
    }
}
