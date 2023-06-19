using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmStateController : MonoBehaviour {

    public ArmStateModelSO currentModel;
    public BooleanSO canThrow;
    public ScriptableObjectListSO availableTablewares;
    public AudioClip emptyThrowClip;
    public AudioSource audioSource;

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
        } else {
            if (Input.GetMouseButtonDown(0)) {
                currentModel.SetToThrowEmpty();
                audioSource.clip = emptyThrowClip;
                audioSource.volume = 1f;
                audioSource.pitch = Random.Range(0.8f, 1.2f);
                audioSource.Play();
            }
        }

        if (!currentModel.IsIdle()) {//When not in idle state, inspect special cases where to return in idle.
            bool throwHasFinished = currentModel.IsThrow() && !currentModel.IsThrowAnimationOn();//in throw state, but animation is over => return to idle
            bool throwEmptyHasFinished = currentModel.IsThrowEmpty() && !currentModel.IsThrowEmptyAnimationOn();//in empty state, but animation is over => return to idle
            bool changingWeaponFromThrow = currentModel.IsThrow() && availableTablewares.cursor != lastCursor;//abruptly changing weapon while throwing => break the throw animation and return to idle.
            bool changingWeaponFromEmpty = currentModel.IsThrowEmpty() && availableTablewares.cursor != lastCursor;//abruptly changing weapon while showing empty => break the throw animation and return to idle.
            if (throwHasFinished || throwEmptyHasFinished || changingWeaponFromThrow || changingWeaponFromEmpty) {
                currentModel.SetToIdle();
            }
        }


        lastCursor = availableTablewares.cursor;
    }
}
