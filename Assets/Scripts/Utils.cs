using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {
    public static void EnablePhysics(GameObject obj, bool e) {
        if (obj.GetComponent<Collider>() != null)
            obj.GetComponent<Collider>().enabled = e;
        foreach (Collider collider in obj.GetComponentsInChildren<Collider>()) {
            collider.enabled = e;
        }

        if (obj.GetComponent<Rigidbody>() != null)
            obj.GetComponent<Rigidbody>().isKinematic = !e;
        foreach (Rigidbody rgbd in obj.GetComponentsInChildren<Rigidbody>()) {
            rgbd.isKinematic = !e;
        }
    }

    /// <summary>
    /// Return the specified vector relatively to the specified transform.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 GetVectorInLocalSpace(Vector3 v, Transform t) {

        return v.x * t.right + v.y * t.up + v.z * t.forward;
    }
}
