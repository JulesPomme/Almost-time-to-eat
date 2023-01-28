using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmStateController : MonoBehaviour {

    public ArmStateModelSO currentModel;
    public BooleanSO canThrow;
    public ScriptableObjectListSO availableTablewares;

    private int lastCursor;

    private void Start() {
        currentModel.TellControllerIsStarted();
        currentModel.SetToIdle();
        lastCursor = -1;
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

        bool throwHasFinished = currentModel.IsThrow() && !currentModel.IsThrowAnimationOn();
        bool changingWeapon = currentModel.IsThrow() && availableTablewares.cursor != lastCursor;
        if (canThrow.no || throwHasFinished || changingWeapon) {
            currentModel.SetToIdle();
        }

        lastCursor = availableTablewares.cursor;
    }
}
