using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This MonoBehaviour ensures an object to be stabilized quickly as soons as it's almost stopped. In other words, it prevents the object from infinite micro-mouvements.
/// </summary>
public class StabilizeObject : MonoBehaviour
{
    private Rigidbody rgbd;

    //To make sure we check only when velocity is decreasing
    private float prevVelocityMagnitude;

    void Start() {
        rgbd = GetComponent<Rigidbody>();
        prevVelocityMagnitude = 0;
    }

    void Update() {
        if ((rgbd.velocity).magnitude < 0.05f && (prevVelocityMagnitude > rgbd.velocity.magnitude)) {
            rgbd.velocity = Vector3.zero;
            rgbd.angularVelocity = Vector3.zero;
            rgbd.Sleep();
        }
        prevVelocityMagnitude = rgbd.velocity.magnitude;
    }
}
