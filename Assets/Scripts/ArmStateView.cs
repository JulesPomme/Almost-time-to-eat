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
        if (currentModel.StateHasChanged()) {
            if (currentModel.IsIdle()) {
                animator.ResetTrigger("hold");
                animator.ResetTrigger("throw");
                animator.ResetTrigger("empty");
                animator.SetTrigger("idle");
            } else if (currentModel.IsHold()) {
                animator.ResetTrigger("idle");
                animator.ResetTrigger("throw");
                animator.ResetTrigger("empty");
                animator.SetTrigger("hold");
            } else if (currentModel.IsThrow()) {
                animator.ResetTrigger("idle");
                animator.ResetTrigger("hold");
                animator.ResetTrigger("empty");
                animator.SetTrigger("throw");
            } else if (currentModel.IsThrowEmpty()) {
                animator.ResetTrigger("idle");
                animator.ResetTrigger("hold");
                animator.ResetTrigger("throw");
                animator.SetTrigger("empty");
            }
        }

        if (currentModel.IsIdle() || currentModel.IsHold()) {
            HandleTablewareInHand();
        }
    }

    private void HandleTablewareInHand() {
        if (tablewareInHand.obj == null) {
            CreateTablewareInHand();
        } else if (availableTablewares.cursor != lastCursor) {
            Destroy(tablewareInHand.obj);
            tablewareInHand.reference = null;
            CreateTablewareInHand();
            lastCursor = availableTablewares.cursor;
        } else if (currentModel.StateHasChanged()) {
            UpdateTablewareInHandTransform();
        }
    }

    private void UpdateTablewareInHandTransform() {
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
        if (tableware.ammo > 0) {
            GameObject clone = Instantiate(tableware.prefab);
            clone.name += Utils.GetUniqueId();
            clone.transform.parent = transform;
            Utils.SetLayerRecursively(clone, gameObject.layer);
            Utils.EnablePhysics(clone, false);
            tablewareInHand.obj = clone;
            UpdateTablewareInHandTransform();
        }
        tablewareInHand.reference = (TablewareSO)availableTablewares.list[availableTablewares.cursor];
    }

    /// <summary>
    /// Called by the throw animation as soon as it is finished.
    /// </summary>
    public void ThrowAnimationIsFinished() {
        currentModel.AlertThrowAnimationIsFinished();
    }

    public void ThrowEmptyAnimationIsFinished() {
        currentModel.AlertThrowEmptyAnimationIsFinished();
    }
}
