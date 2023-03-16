using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTablewareRegister : MonoBehaviour {

    public TablewareInstanceListSO instantiatedTableware;

    private void OnTriggerEnter(Collider other) {

        TablewareInstanceListSO.Container? container = instantiatedTableware.FindContainerWithCollider(other);
        if (container.HasValue) {
            GameObject owner = instantiatedTableware.FindOwner(container.Value.instance);
            if (owner != gameObject) {
                instantiatedTableware.Remove(container.Value.instance);
                instantiatedTableware.Add(gameObject, container.Value);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        Debug.Log(other.name + " exit");
        TablewareInstanceListSO.Container? container = instantiatedTableware.FindContainerWithCollider(other);
        if (container.HasValue) {
            GameObject owner = instantiatedTableware.FindOwner(container.Value.instance);
            if (owner == gameObject) {
                instantiatedTableware.Remove(container.Value.instance);
                instantiatedTableware.Add(instantiatedTableware.GetDefaultOwner(), container.Value);
            }
        }
    }
}
