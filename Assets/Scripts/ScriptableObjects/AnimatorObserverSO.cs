using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AnimatorObserver")]
public class AnimatorObserverSO : ScriptableObject {
    public enum State {
        NONE, STARTED, FINISHED
    }

    private Dictionary<GameObject, Dictionary<string, State>> stateRegister = new Dictionary<GameObject, Dictionary<string, State>>();

    public void Clear() {
        stateRegister.Clear();
    }

    public void SetState(GameObject obj, string animName, State state) {
        if (!stateRegister.ContainsKey(obj)) {
            stateRegister[obj] = new Dictionary<string, State>();
        }
        stateRegister[obj][animName] = state;
    }

    public State GetState(GameObject obj, string animName) {
        State res = State.NONE;
        if (stateRegister.ContainsKey(obj) && stateRegister[obj].ContainsKey(animName))
            res = stateRegister[obj][animName];
        return res;
    }

    public void StopObserving(GameObject obj) {
        stateRegister.Remove(obj);
    }
}
