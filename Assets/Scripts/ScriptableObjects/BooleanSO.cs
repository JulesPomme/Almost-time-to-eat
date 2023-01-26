using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Bool")]
public class BooleanSO : ScriptableObject
{
    public bool yes;
    public bool no { get { return !yes; } }
}
