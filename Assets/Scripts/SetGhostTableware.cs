using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGhostTableware : MonoBehaviour {

    public Transform basePosition;

    private TablewareZoneSO zoneSO;
    private TableSO tableSO;
    private Transform tableInstanceTransform;

    void Start() {
        GameObject ghost = Instantiate<GameObject>(zoneSO.tableware.ghostPrefab);
        ghost.transform.parent = transform;
        ghost.transform.localPosition = new Vector3(0, basePosition.localPosition.y + 0.01f, 0);
        Vector3? anchor = GetOrientationAnchor(tableSO, zoneSO);
        if (anchor.HasValue) {
            Vector3 anchorWorldPos = Utils.GetVectorInLocalSpace(anchor.Value, tableInstanceTransform) + tableInstanceTransform.position; //anchor position in world space
            Quaternion rotationToAnchor = Quaternion.LookRotation(anchorWorldPos - ghost.transform.position, Vector3.up);
            ghost.transform.rotation = rotationToAnchor;
        }
    }

    public void SetZoneSO(TablewareZoneSO z) {
        zoneSO = z;
    }

    public void SetTableSO(TableSO t) {
        tableSO = t;
    }

    public void SetTableInstanceTransform(Transform t) {
        tableInstanceTransform = t;
    }

    /// <summary>
    /// Return the anchor this ghost must rotate towards. If no anchor exists, return null.
    /// </summary>
    /// <param name="table"></param>
    /// <param name="zoneSO"></param>
    /// <returns></returns>
    private Vector3? GetOrientationAnchor(TableSO table, TablewareZoneSO zoneSO) {

        Vector3? res = null;
        TableSO.OrientationBinder[] binders = table.orientationBinders;
        int i = 0;
        while (!res.HasValue && i < binders.Length) {
            int j = 0;
            TablewareZoneSO[] compasses = binders[i].compasses;
            while (!res.HasValue && j < compasses.Length) {
                if (compasses[j] == zoneSO) {
                    res = binders[i].anchor;
                }
                j++;
            }
            i++;
        }
        return res;
    }
}
