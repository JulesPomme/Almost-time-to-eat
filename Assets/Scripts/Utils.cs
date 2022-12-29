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

    public static float GetAngleAlongX(Vector3 from, Vector3 to) {
        Vector3 fromX = new Vector3(0, from.y, from.z);
        Vector3 toX = new Vector3(0, to.y, to.z);
        //Debug.Log("## Angle X from " + fromX + " to " + toX + " is " + Vector3.SignedAngle(fromX, toX, Vector3.right));
        return (fromX.magnitude < 0.001 || toX.magnitude < 0.001) ? 0 : Vector3.SignedAngle(fromX, toX, Vector3.right);
    }

    public static float GetAngleAlongY(Vector3 from, Vector3 to) {
        Vector3 fromY = new Vector3(from.x, 0, from.z);
        Vector3 toY = new Vector3(to.x, 0, to.z);
        //Debug.Log("## Angle Y from " + fromY + " to " + toY + " is " + Vector3.SignedAngle(fromY, toY, Vector3.up));
        return (fromY.magnitude < 0.001 || toY.magnitude < 0.001) ? 0 : Vector3.SignedAngle(fromY, toY, Vector3.up);
    }

    public static float GetAngleAlongZ(Vector3 from, Vector3 to) {
        Vector3 fromZ = new Vector3(from.x, from.y, 0);
        Vector3 toZ = new Vector3(to.x, to.y, 0);
        //Debug.Log("## Angle Z from " + fromZ + " to " + toZ + " is " + Vector3.SignedAngle(fromZ, toZ, Vector3.forward));
        return (fromZ.magnitude < 0.001 || toZ.magnitude < 0.001) ? 0 : Vector3.SignedAngle(fromZ, toZ, Vector3.forward);
    }

    public static Vector3 GetAngleVec(Vector3 from, Vector3 to) {
        Vector3 fromX = new Vector3(0, from.y, from.z);
        Vector3 toX = new Vector3(0, to.y, to.z);
        float angleX = (fromX.magnitude < 0.001 || toX.magnitude < 0.001) ? 0 : Vector3.SignedAngle(fromX, toX, Vector3.right);

        Vector3 fromY = new Vector3(from.x + angleX, 0, from.z);
        Vector3 toY = new Vector3(to.x, 0, to.z);
        float angleY = (fromY.magnitude < 0.001 || toY.magnitude < 0.001) ? 0 : Vector3.SignedAngle(fromY, toY, Vector3.up);

        Vector3 fromZ = new Vector3(from.x + angleX, from.y + angleY, 0);
        Vector3 toZ = new Vector3(to.x, to.y, 0);
        float angleZ = (fromZ.magnitude < 0.001 || toZ.magnitude < 0.001) ? 0 : Vector3.SignedAngle(fromZ, toZ, Vector3.forward);

        return new Vector3(angleX, angleY, angleZ);
    }
}
