using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Table")]
public class TableSO : ScriptableObject {
    public List<GameObject> instances;
    public ScriptableObjectListSO waveList;
}
