using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnchorCompassRotation : MonoBehaviour {

    public Transform anchor;
    //public Transform compass;
    public Transform obj;

    void Start() {

    }

    void Update() {
        Quaternion myRotation = Quaternion.LookRotation(anchor.position - obj.position, Vector3.up);
        obj.transform.rotation = myRotation;
    }
}
