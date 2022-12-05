using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/GameObject List")]
public class GameObjectListSO : ScriptableObject
{
    public List<GameObject> list;
}
