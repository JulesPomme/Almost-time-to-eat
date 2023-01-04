using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleArmAnimation : MonoBehaviour {

    public ArmStateHandler currentArmState;
    public ScriptableObjectListSO availableTablewares;
    public TablewareInstanceSO tablewareInHand;
    public Vector2 holdTablewareOffset;

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
        TablewareSO tableware = ((TablewareSO)availableTablewares.list[availableTablewares.cursor]);
        GameObject clone = Instantiate(tableware.prefab);
        clone.transform.parent = transform;
        clone.transform.localPosition = tableware.idleLocalPosition;
        clone.transform.localEulerAngles = tableware.idleLocalOrientation;
        Utils.EnablePhysics(clone, false);
        tablewareInHand.obj = clone;
        tablewareInHand.reference = (TablewareSO)availableTablewares.list[availableTablewares.cursor];
    }

    /// <summary>
    /// Called by the throw animation as soon as it is finished.
    /// </summary>
    public void ThrowAnimationIsFinished() {
        currentArmState.AlertThrowAnimationIsFinished();
    }

    /// <summary>
    /// Called by the hold animation as soon as it is started.
    /// </summary>
    public void HoldAnimationIsStarted() {
        tablewareInHand.obj.transform.localPosition = tablewareInHand.obj.transform.localPosition - (holdTablewareOffset.y * tablewareInHand.obj.transform.forward) - (holdTablewareOffset.x * tablewareInHand.obj.transform.right);
    }
}
