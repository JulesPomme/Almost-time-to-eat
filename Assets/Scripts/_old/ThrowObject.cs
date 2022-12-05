using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    public TablewareZoneSO currentWave;
    public FloatSO throwingForceMin;
    public FloatSO throwingForceMax;
    public FloatSO throwingForce;
    public float forceVariationSpeed;
    public GameObjectListSO instantiatedTablewares;
    public GameObjectSO currentlyThrown;
    public AudioSource woosh;
    public Vector3SO throwingDirection;
    public FloatSO currentTablewareOrientation;

    private bool ascending;

    void Start() {
        ResetThrowingForce();
    }

    void Update() {

        //Checking if mouse is pointing at the table (even through other objects)
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        bool mouseIsPointingTable = false;
        int i = 0;
        while (!mouseIsPointingTable && i < hits.Length) {
            RaycastHit hit = hits[i];
            if (hit.transform.tag == "Table") {
                mouseIsPointingTable = true;
            } else {
                i++;
            }
        }

        //If yes, then adjust throwing force at mouse clicking and throw an object at mouse releasing
        if (mouseIsPointingTable) {
            RaycastHit hit = hits[i];
            throwingDirection.value = (hit.point - transform.position).normalized;
            if (Input.GetMouseButton(0)) {
                if (ascending) {
                    throwingForce.value += Time.deltaTime * forceVariationSpeed;
                } else {
                    throwingForce.value -= Time.deltaTime * forceVariationSpeed;
                }
                throwingForce.value = Mathf.Clamp(throwingForce.value, throwingForceMin.value, throwingForceMax.value);
                if (throwingForce.value == throwingForceMin.value || throwingForce.value == throwingForceMax.value) {
                    ascending = !ascending;
                }
            }
            if (Input.GetMouseButtonUp(0)) {
                GameObject clone = Instantiate<GameObject>(currentWave.tableware.prefab);
                clone.transform.position = transform.position;
                clone.transform.eulerAngles = new Vector3(clone.transform.eulerAngles.x, currentTablewareOrientation.value, clone.transform.eulerAngles.z);
                clone.transform.parent = transform;
                clone.GetComponent<Rigidbody>().AddForce(throwingDirection.value * throwingForce.value, ForceMode.Impulse);
                instantiatedTablewares.list.Add(clone);
                currentlyThrown.obj = GetObjectWithCollider(clone);
                ResetThrowingForce();
                woosh.pitch = Random.Range(0.8f, 1.2f);
                woosh.Play();
            }
        } else { //If no, reset throwingForce (in case player clicks on the mouse button while ON the table, and releases it while OFF the table)
            ResetThrowingForce();
        }
    }

    private GameObject GetObjectWithCollider(GameObject clone) {
        GameObject res = null;
        if (clone.GetComponent<Collider>() != null && !clone.GetComponent<Collider>().isTrigger) {
            res = clone;
        } else {
            foreach (Transform child in clone.transform) {
                if (child.GetComponent<Collider>() != null && !child.GetComponent<Collider>().isTrigger) {
                    res = child.gameObject;
                    break;
                }
            }
        }
        return res;
    }

    private void ResetThrowingForce() {
        ascending = true;
        throwingForce.value = throwingForceMin.value;
    }
}
