using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Tableware Zone")]
public class TablewareZoneSO : ScriptableObject
{
    public TablewareSO tableware;
    public GameObject zonePrefab;
    public Vector3 zoneLocalPosition;
}
