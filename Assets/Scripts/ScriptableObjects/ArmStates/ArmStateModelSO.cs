using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ArmStateDescription")]
public class ArmStateModelSO : ScriptableObject {

    public ArmStateSO idleState;
    public ArmStateSO holdState;
    public ArmStateSO throwState;
    public ArmStateSO throwEmptyState;
    private ArmStateSO currentArmState;
    private ArmStateSO previousArmState;

    private bool isControllerStarted;
    private bool throwAnimationIsOn;
    private bool throwEmptyAnimationIsOn;

    public void Init() {
        previousArmState = null;
        isControllerStarted = false;
        throwAnimationIsOn = false;
    }

    public void SetToIdle() {
        currentArmState = idleState;
        throwAnimationIsOn = false;
        throwEmptyAnimationIsOn = false;
    }

    public void SetToHold() {
        currentArmState = holdState;
        throwAnimationIsOn = false;
        throwEmptyAnimationIsOn = false;
    }

    public void SetToThrow() {
        currentArmState = throwState;
        throwAnimationIsOn = true;
        throwEmptyAnimationIsOn = false;
    }

    public void SetToThrowEmpty() {
        currentArmState = throwEmptyState;
        throwAnimationIsOn = false;
        throwEmptyAnimationIsOn = true;
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

    public bool IsThrowEmpty() {
        return currentArmState == throwEmptyState;
    }

    public void AlertThrowAnimationIsFinished() {
        throwAnimationIsOn = false;
    }

    public void AlertThrowEmptyAnimationIsFinished() {
        throwEmptyAnimationIsOn = false;
    }

    public bool IsThrowAnimationOn() {
        return throwAnimationIsOn;
    }

    public bool IsThrowEmptyAnimationOn() {
        return throwEmptyAnimationIsOn;
    }

    public void PrintCurrentState() {
        if (currentArmState == idleState) {
            Debug.Log("### Arm state is idle");
        } else if (currentArmState == holdState) {
            Debug.Log("### Arm state is hold");
        } else if (currentArmState == throwState) {
            Debug.Log("### Arm state is throw");
        } else if (currentArmState == throwEmptyState) {
            Debug.Log("### Arm state is throw empty");
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
