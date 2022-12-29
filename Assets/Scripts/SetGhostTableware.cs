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
        Vector3[] binder = GetOrientationBinder(tableSO, zoneSO);
        if (binder != null) {
            Vector3 anchor = Utils.GetVectorInLocalSpace(binder[0], tableInstanceTransform) + tableInstanceTransform.position; //anchor position in world space
            Vector3 compass = Utils.GetVectorInLocalSpace(binder[1], ghost.transform) + ghost.transform.position; //compass position in world space
            float angleX = Utils.GetAngleAlongX(compass - ghost.transform.position, anchor - ghost.transform.position);
            ghost.transform.eulerAngles = new Vector3(ghost.transform.eulerAngles.x + angleX, ghost.transform.eulerAngles.y, ghost.transform.eulerAngles.z);
            compass = Utils.GetVectorInLocalSpace(binder[1], ghost.transform) + ghost.transform.position; //new compass position in world space after X rotation of ghost
            float angleY = Utils.GetAngleAlongY(compass - ghost.transform.position, anchor - ghost.transform.position);
            ghost.transform.eulerAngles = new Vector3(ghost.transform.eulerAngles.x, ghost.transform.eulerAngles.y + angleY, ghost.transform.eulerAngles.z);
            compass = Utils.GetVectorInLocalSpace(binder[1], ghost.transform) + ghost.transform.position; //new compass position in world space after Y rotation of ghost
            float angleZ = Utils.GetAngleAlongZ(compass - ghost.transform.position, anchor - ghost.transform.position);
            ghost.transform.eulerAngles = new Vector3(ghost.transform.eulerAngles.x, ghost.transform.eulerAngles.y, ghost.transform.eulerAngles.z + angleZ);
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
    /// Return the orientation binder between a table ScriptableObject and a TablewareZone ScriptableObject.
    /// A binder is an array of two Vector3: the first one belongs to the table, and is the orientation anchor, i.e., the anchor that orients an encompass.
    /// The second one belongs to the tableware and is the orientation encompass, i.e., the object that is oriented by an anchor.
    /// </summary>
    /// <param name="table"></param>
    /// <param name="zoneSO"></param>
    /// <returns></returns>
    private Vector3[] GetOrientationBinder(TableSO table, TablewareZoneSO zoneSO) {

        Vector3[] res = null;
        TableSO.OrientationBinder[] binders = table.orientationBinders;
        int i = 0;
        while (res == null && i < binders.Length) {
            int j = 0;
            TablewareZoneSO[] compasses = binders[i].compasses;
            while (res == null && j < compasses.Length) {
                if (compasses[j] == zoneSO) {
                    res = new Vector3[2];
                    res[0] = binders[i].anchor;
                    res[1] = compasses[j].tableware.orientationCompass;
                }
                j++;
            }
            i++;
        }
        return res;
    }
}
