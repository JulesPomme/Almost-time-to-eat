using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ArmStateDescription")]
public class ArmStateHandlerSO : ScriptableObject {

    public ArmStateSO idleState;
    public ArmStateSO holdState;
    public ArmStateSO throwState;
    private ArmStateSO currentArmState;

    private bool throwAnimationIsOn = false;

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
}
