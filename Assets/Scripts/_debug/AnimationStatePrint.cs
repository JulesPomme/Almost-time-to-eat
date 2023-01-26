using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStatePrint : StateMachineBehaviour {
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.Log(stateInfo.shortNameHash + " State enter");
        Debug.Log("state is looping " + stateInfo.loop);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.Log(stateInfo.shortNameHash + " State exit");
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.Log(stateInfo.shortNameHash + " State move");
    }
}
