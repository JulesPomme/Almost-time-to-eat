using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/SO List")]
public class ScriptableObjectListSO : ScriptableObject
{
    public List<ScriptableObject> list;

    public int cursor;
}
