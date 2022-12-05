using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Physical GameObject")]
public class PhysicalGameObjectSO : ScriptableObject
{
    public GameObject gameObject;
    public GameObject ghost;
}
