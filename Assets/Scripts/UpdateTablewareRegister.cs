using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTablewareRegister : MonoBehaviour {

    public TablewareInstanceListSO instantiatedTableware;
    public GameObject table;

    private void OnTriggerEnter(Collider other) {

        TablewareInstanceListSO.Container? container = instantiatedTableware.FindContainerWithCollider(other);
        if (container.HasValue) {
            GameObject owner = instantiatedTableware.FindOwner(container.Value.instance);
            if (owner != table) {
                instantiatedTableware.Remove(container.Value.instance);
                instantiatedTableware.Add(table, container.Value);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        TablewareInstanceListSO.Container? container = instantiatedTableware.FindContainerWithCollider(other);
        if (container.HasValue) {
            GameObject owner = instantiatedTableware.FindOwner(container.Value.instance);
            if (owner == table) {
                instantiatedTableware.Remove(container.Value.instance);
                instantiatedTableware.Add(instantiatedTableware.GetDefaultOwner(), container.Value);
            }
        }
    }
}
