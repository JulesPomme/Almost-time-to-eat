using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Tableware instance")]
public class TablewareInstanceSO : ScriptableObject
{
    public GameObject obj;
    public TablewareSO reference;
}
