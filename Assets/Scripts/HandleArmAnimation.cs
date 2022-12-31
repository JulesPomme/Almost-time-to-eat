using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleArmAnimation : MonoBehaviour {

    public ArmStateHandler currentArmState;
    public ScriptableObjectListSO availableTablewares;
    public Vector3SO idlePoint;
    public TablewareInstanceSO tablewareInHand;

    private Animator animator;
    private int lastCursor;

    void Start() {
        animator = GetComponent<Animator>();
        lastCursor = availableTablewares.cursor;
    }

    void Update() {

        if (currentArmState.IsHold()) {
            HandleTablewareCreation();
            animator.SetBool("hold", true);
        } else if (currentArmState.IsThrow()) {
            animator.SetBool("hold", false);
        }

        if (currentArmState.IsIdle()) {
            HandleTablewareCreation();
        }
    }

    private void HandleTablewareCreation() {
        if (tablewareInHand.obj == null) {
            CreateTablewareInHand();
        } else if (availableTablewares.cursor != lastCursor) {
            Destroy(tablewareInHand.obj);
            tablewareInHand.reference = null;
            CreateTablewareInHand();
            lastCursor = availableTablewares.cursor;
        }
    }

    private void CreateTablewareInHand() {
        GameObject clone = Instantiate<GameObject>(((TablewareSO)availableTablewares.list[availableTablewares.cursor]).prefab);
        clone.transform.parent = transform;
        clone.transform.position = idlePoint.value;
        clone.transform.localRotation = Quaternion.identity;
        Utils.EnablePhysics(clone, false);
        tablewareInHand.obj = clone;
        tablewareInHand.reference = (TablewareSO)availableTablewares.list[availableTablewares.cursor];
    }

    public void ThrowAnimationIsFinished() {
        currentArmState.AlertThrowAnimationIsFinished();
    }
}
