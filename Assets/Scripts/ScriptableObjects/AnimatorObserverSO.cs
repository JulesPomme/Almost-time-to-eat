using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AnimatorObserver")]
public class AnimatorObserverSO : ScriptableObject {
    public enum State {
        NONE, STARTED, FINISHED
    }

    private Dictionary<string, State> stateRegister = new Dictionary<string, State>();

    public void Clear() {
        stateRegister.Clear();
    }

    public void SetState(string animName, State state) {
        stateRegister[animName] = state;
    }

    public State GetState(string animName) {

        return stateRegister.ContainsKey(animName) ? stateRegister[animName] : State.NONE;
    }
}
