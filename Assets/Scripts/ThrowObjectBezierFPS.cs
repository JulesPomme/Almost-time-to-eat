using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObjectBezierFPS : MonoBehaviour {
    public Vector3SO throwingPoint;
    public FloatSO throwingForceMax;
    public TablewareInstanceListSO instantiatedTablewares;
    public AudioSource woosh;
    public Vector3SO throwingDirection;
    public float throwingDuration = 1f;
    public bool showTarget = true;
    public GameObject targetPrefab;
    public ScriptableObjectListSO tableList;
    public ScriptableObjectListSO availableTablewares;

    private GameObject target;

    void Start() {
        target = Instantiate<GameObject>(targetPrefab);
        target.transform.parent = transform;
        target.SetActive(showTarget);
    }

    void Update() {
        target.SetActive(showTarget);
        //Get the direction of the object to throw by looking at what the crosshair is pointing
        Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        RaycastHit hit;
        //If the player is pointing something...
        if (Physics.Raycast(ray, out hit)) {
            //...compute and show the target point if required...
            target.transform.forward = hit.normal;
            //put the target on the hit point + a small amount above, in order to always keep it visible.
            target.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z) + (target.transform.forward / 10f);

            if (Input.GetMouseButtonUp(0)) {//...and throw an object at mouse releasing...
                GameObject clone = Instantiate<GameObject>(((TablewareSO)availableTablewares.list[availableTablewares.cursor]).prefab);
                clone.transform.position = throwingPoint.value;
                GameObject tableTopSurface = IsUponTableTopSurface(target);
                if (tableTopSurface != null) {
                    float orientation = clone.transform.eulerAngles.y + Vector3.SignedAngle(Vector3.right, hit.point - tableTopSurface.transform.position, Vector3.up);
                    clone.transform.eulerAngles = new Vector3(clone.transform.eulerAngles.x, orientation, clone.transform.eulerAngles.z);
                }
                clone.transform.parent = transform;

                StartCoroutine(ThrowObjectWithBezierCoroutine(clone, hit.point));

                TablewareInstanceListSO.Container container = new TablewareInstanceListSO.Container();
                container.instance = clone;
                container.reference = ((TablewareSO)availableTablewares.list[availableTablewares.cursor]);
                container.objectsWithColliders = GetObjectsWithColliders(clone);
                instantiatedTablewares.list.Add(container);
                woosh.pitch = Random.Range(0.8f, 1.2f);
                woosh.Play();
            }
        } else { //If the player is pointing nothing, hide the target
            target.SetActive(false);
        }
    }

    private GameObject IsUponTableTopSurface(GameObject target) {
        GameObject topSurface = null;
        RaycastHit[] hits = Physics.RaycastAll(target.transform.position, Vector3.down);
        //Debug.Log("Under there are " + hits.Length + " objects with a collider");
        //Debug.DrawRay(obj.transform.position, Vector3.down, Color.red);
        //Debug.Break();
        int i = 0;
        while (topSurface == null && i < hits.Length) {
            if (IsTableTopSurface(hits[i].collider.gameObject)) {
                topSurface = hits[i].collider.gameObject;
            }
            //Debug.Log("\t Checking if " + hits[i].collider.gameObject.name + " is a table top surface = " + res);
            i++;
        }
        return topSurface;
    }

    private bool IsTableTopSurface(GameObject obj) {

        bool res = false;
        int i = 0;
        while (!res && i < tableList.list.Count) {
            int j = 0;
            while (!res && j < ((TableSO)tableList.list[i]).instances.Count) {
                GameObject tableInstance = ((TableSO)tableList.list[i]).instances[j];
                if (tableInstance != null) {
                    res = tableInstance.GetComponent<TableHandler>().topSurface == obj;
                }
                j++;
            }
            i++;
        }
        return res;
    }

    private IEnumerator ThrowObjectWithBezierCoroutine(GameObject clone, Vector3 targetPoint) {

        EnablePhysics(clone, false);
        float t = 0;
        Vector3 initPoint = new Vector3(clone.transform.position.x, clone.transform.position.y, clone.transform.position.z);
        Vector3 p1 = initPoint + ((targetPoint - initPoint) / 2f) + new Vector3(0, 2, 0);
        while (t < throwingDuration) {
            if (clone == null) {
                //This can happen if the player resets the game while this object is being thrown. If so, end the coroutine now.
                yield break;
            }
            float ratio = t / throwingDuration;
            clone.transform.position = CalculateQuadraticBezierPoint(ratio, initPoint, p1, targetPoint);
            yield return new WaitForSeconds(Time.deltaTime);
            t += Time.deltaTime;
        }
        if (clone != null) {
            //This can happen if the player resets the game while this object is being thrown. If so, end the coroutine now.
            clone.transform.position = CalculateQuadraticBezierPoint(1, initPoint, p1, targetPoint);
            EnablePhysics(clone, true);
        }
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2) {
        //B(t) = (1-t)^2P0 + 2(t-1)tP1 + t^2P2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }

    private List<GameObject> GetObjectsWithColliders(GameObject clone) {
        List<GameObject> res = new List<GameObject>();
        if (clone.GetComponent<Collider>() != null && !clone.GetComponent<Collider>().isTrigger) {
            res.Add(clone);
        }
        foreach (Collider collider in clone.GetComponentsInChildren<Collider>()) {
            if (!collider.isTrigger) {
                res.Add(collider.gameObject);
            }
        }
        return res;
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
