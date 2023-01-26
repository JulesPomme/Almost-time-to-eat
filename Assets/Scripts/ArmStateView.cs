using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmStateView : MonoBehaviour {

    public ArmStateModelSO currentModel;
    public ScriptableObjectListSO availableTablewares;
    public TablewareInstanceSO tablewareInHand;

    private Animator animator;
    private int lastCursor;

    private void Awake() {
        currentModel.Init();
    }

    void Start() {
        animator = GetComponent<Animator>();
        lastCursor = availableTablewares.cursor;
    }

    void Update() {


        if (!currentModel.IsControllerStarted())
            return;
        Debug.Log(animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).shortNameHash);
        if (currentModel.StateHasChanged()) {
            if (currentModel.IsIdle()) {
                animator.ResetTrigger("hold");
                animator.ResetTrigger("throw");
                animator.SetTrigger("idle");
            } else if (currentModel.IsHold()) {
                animator.ResetTrigger("idle");
                animator.ResetTrigger("throw");
                animator.SetTrigger("hold");
            } else if (currentModel.IsThrow()) {
                animator.ResetTrigger("idle");
                animator.ResetTrigger("hold");
                animator.SetTrigger("throw");
            }
        }

        if (currentModel.IsIdle() || currentModel.IsHold()) {
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
        } else if (currentModel.StateHasChanged()) {
            UpdateTablewareTransform();
        }
    }

    private void UpdateTablewareTransform() {
        TablewareSO tableware = ((TablewareSO)availableTablewares.list[availableTablewares.cursor]);
        if (currentModel.IsIdle()) {
            tablewareInHand.obj.transform.localPosition = tableware.idleLocalPosition;
            tablewareInHand.obj.transform.localEulerAngles = tableware.idleLocalOrientation;
        } else if (currentModel.IsHold()) {
            tablewareInHand.obj.transform.localPosition = tableware.holdLocalPosition;
            tablewareInHand.obj.transform.localEulerAngles = tableware.holdLocalRotation;
        }
    }

    private void CreateTablewareInHand() {
        TablewareSO tableware = ((TablewareSO)availableTablewares.list[availableTablewares.cursor]);
        GameObject clone = Instantiate(tableware.prefab);
        clone.transform.parent = transform;
        Utils.EnablePhysics(clone, false);
        tablewareInHand.obj = clone;
        tablewareInHand.reference = (TablewareSO)availableTablewares.list[availableTablewares.cursor];
        UpdateTablewareTransform();
    }

    /// <summary>
    /// Called by the throw animation as soon as it is finished.
    /// </summary>
    public void ThrowAnimationIsFinished() {
        currentModel.AlertThrowAnimationIsFinished();
    }
}
