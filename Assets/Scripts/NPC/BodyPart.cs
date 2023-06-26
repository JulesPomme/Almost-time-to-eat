using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BodyPart : MonoBehaviour {
    [HideInInspector]
    public Rigidbody rgbd;
    private bool isCollidedByTableware;
    private List<GameObject> collisionings;

    public void Awake() {
        rgbd = GetComponent<Rigidbody>();
        isCollidedByTableware = false;
        collisionings = new List<GameObject>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Tableware") {
            isCollidedByTableware = true;
            collisionings.Add(collision.gameObject);
        }
    }

    //private void OnCollisionExit(Collision collision) {
    //    if (collision.gameObject.tag == "Tableware") {
    //        Debug.Log("*" + name + " uncollides with " + collision.gameObject.name);
    //        isCollidedByTableware = false;
    //        collisionings.Remove(collision.gameObject);
    //    }
    //}

    private void Update() {
        int i = collisionings.Count - 1;
        while (i >= 0) {
            if (collisionings[i] == null) {
                collisionings.RemoveAt(i);
            }
            i--;
        }
    }

    public bool IsCollidedByTableware() {
        return isCollidedByTableware;
    }
}
