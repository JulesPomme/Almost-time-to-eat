using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable Objects/Tableware")]
public class TablewareSO : ScriptableObject {
    public GameObject prefab;
    public GameObject ghostPrefab;
    public Sprite selectorIcon;
    public Vector3 idleLocalPosition;
    public Vector3 idleLocalOrientation;
    public Vector3 holdLocalPosition;
    public Vector3 holdLocalRotation;
    public int initAmmo;
    public int ammo;
}
