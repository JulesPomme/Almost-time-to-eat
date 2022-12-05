using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateMass : MonoBehaviour
{

    public GameObjectSO currentlyThrown;

    private bool updated;
    void Start() {
        updated = false;
    }

    void Update() {
        if (currentlyThrown.obj != gameObject && !updated) {
            GetComponent<Rigidbody>().mass = 0.01f;
            updated = true;
        }
    }
}
