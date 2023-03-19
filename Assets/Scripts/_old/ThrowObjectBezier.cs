using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObjectBezier : MonoBehaviour
{
    public TablewareZoneSO currentWave;
    public FloatSO throwingForceMin;
    public FloatSO throwingForceMax;
    public FloatSO throwingForce;
    public float forceVariationSpeed;
    public TablewareInstanceListSO instantiatedTablewares;
    public AudioSource woosh;
    public Vector3SO throwingDirection;
    public float throwingDuration = 1f;
    public bool showTarget = true;
    public GameObject targetPrefab;
    public GameObjectSO tableInstance;

    private bool ascending;
    private GameObject target;

    void Start() {
        ResetThrowingForce();
        target = Instantiate<GameObject>(targetPrefab);
        target.transform.parent = transform;
        target.SetActive(false);
    }

    void Update() {
        //Get the direction of the object to throw by looking at what the crosshair is pointing
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

        //If yes...
        if (mouseIsPointingTable) {//...adjust the throwing force at mouse clicking...
            RaycastHit hit = hits[i];
            Vector3 startPoint = new Vector3(Camera.main.transform.position.x, hit.point.y, Camera.main.transform.position.z);
            throwingDirection.value = (hit.point - startPoint).normalized;
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

            //...show the target point if required...
            Vector3 targetPoint = startPoint + (throwingDirection.value * throwingForce.value);
            target.SetActive(showTarget && throwingForce.value > throwingForceMin.value);
            target.transform.position = new Vector3(targetPoint.x, targetPoint.y + 0.01f, targetPoint.z);

            if (Input.GetMouseButtonUp(0)) {//...and throw an object at mouse releasing...
                GameObject clone = Instantiate<GameObject>(currentWave.tableware.prefab);
                clone.transform.position = Camera.main.transform.position;
                float orientation = clone.transform.eulerAngles.y + Vector3.SignedAngle(tableInstance.obj.transform.right, targetPoint - tableInstance.obj.transform.position, Vector3.up);
                clone.transform.eulerAngles = new Vector3(clone.transform.eulerAngles.x, orientation, clone.transform.eulerAngles.z);
                clone.transform.parent = transform;

                StartCoroutine(ThrowObjectWithBezierCoroutine(clone, targetPoint));

                TablewareInstanceListSO.Container container = new TablewareInstanceListSO.Container();
                container.instance = clone;
                container.reference = currentWave.tableware;
                container.physicalColliders = GetObjectsWithColliders(clone);
                instantiatedTablewares.Add(gameObject, container);
                ResetThrowingForce();
                woosh.pitch = Random.Range(0.8f, 1.2f);
                woosh.Play();
            }
        } else { //If no, reset throwingForce (in case player clicks on the mouse button while ON the table, and releases it while OFF the table)
            ResetThrowingForce();
            target.SetActive(false);
        }
    }

    private IEnumerator ThrowObjectWithBezierCoroutine(GameObject clone, Vector3 targetPoint) {

        EnablePhysics(clone, false);
        float t = 0;
        Vector3 initPoint = new Vector3(clone.transform.position.x, clone.transform.position.y, clone.transform.position.z);
        Vector3 p1 = initPoint + ((targetPoint - initPoint) / 2f) + new Vector3(0, 2, 0);
        while (t < throwingDuration) {
            float ratio = t / throwingDuration;
            clone.transform.position = CalculateQuadraticBezierPoint(ratio, initPoint, p1, targetPoint);
            yield return new WaitForSeconds(Time.deltaTime);
            t += Time.deltaTime;
        }
        clone.transform.position = CalculateQuadraticBezierPoint(1, initPoint, p1, targetPoint);
        EnablePhysics(clone, true);
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2) {
        //B(t) = (1-t)^2P0 + 2(t-1)tP1 + t^2P2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }

    private List<Collider> GetObjectsWithColliders(GameObject clone) {
        List<Collider> res = new List<Collider>();
        if (clone.GetComponent<Collider>() != null && !clone.GetComponent<Collider>().isTrigger) {
            res.Add(clone.GetComponent<Collider>());
        }
        foreach (Collider collider in clone.GetComponentsInChildren<Collider>()) {
            if (!collider.isTrigger) {
                res.Add(collider);
            }
        }
        return res;
    }

    private void ResetThrowingForce() {
        ascending = true;
        throwingForce.value = throwingForceMin.value;
    }

    private void EnablePhysics(GameObject obj, bool e) {
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
}
