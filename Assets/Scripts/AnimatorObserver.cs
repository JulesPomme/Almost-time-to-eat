using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorObserver : MonoBehaviour
{
    public AnimatorObserverSO observer;

    private void Start() {
        observer.Clear();
    }

    public void AlertAnimationIsFinished(string animName) {
        observer.SetState(animName, AnimatorObserverSO.State.FINISHED);
    }

    public void AlertAnimationIsStarted(string animName) {
        observer.SetState(animName, AnimatorObserverSO.State.STARTED);
    }
}
