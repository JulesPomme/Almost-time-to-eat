using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Table")]
public class TableSO : ScriptableObject {
    public List<GameObject> instances;
    public ScriptableObjectListSO waveList;

    [System.Serializable]
    public struct OrientationBinder {
        public Vector3 anchor;
        public TablewareZoneSO[] compasses;
    }

    public OrientationBinder[] orientationBinders;
    public int howLongToRefreshInSeconds;
}
