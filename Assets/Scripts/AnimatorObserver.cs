using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorObserver : MonoBehaviour
{
    public AnimatorObserverSO observer;

    public void AlertAnimationIsFinished(string animName) {
        observer.SetState(gameObject, animName, AnimatorObserverSO.State.FINISHED);
    }

    public void AlertAnimationIsStarted(string animName) {
        observer.SetState(gameObject, animName, AnimatorObserverSO.State.STARTED);
    }
}
