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
    public TablewareInstanceSO tablewareInHand;

    private GameObject target;

    private struct TableStruct {
        public TableSO reference;
        public GameObject instance;
    }

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
                GameObject tw = tablewareInHand.obj;
                tw.transform.position = throwingPoint.value; //TODO quand on lance les couverts un peu trop frénétiquement, le tablewareInHand peut parfois être null au moment où on arrive ici.
                TableStruct? tableStruct = GetTableUnder(target);
                if (tableStruct.HasValue) {
                    //Rotate the tableware towards the anchor if exists
                    Vector3? anchor = GetAnchorWithNearestCompass(tableStruct.Value, target);
                    if (anchor != null) {
                        Quaternion rotationToAnchor = Quaternion.LookRotation(anchor.Value - target.transform.position, Vector3.up);
                        tw.transform.rotation = rotationToAnchor;
                    }
                }
                tw.transform.parent = transform.parent;

                StartCoroutine(ThrowObjectWithBezierCoroutine(tw, hit.point));

                TablewareInstanceListSO.Container container = new TablewareInstanceListSO.Container();
                container.instance = tw;
                container.reference = (TablewareSO)availableTablewares.list[availableTablewares.cursor];
                container.objectsWithColliders = GetChildrenWithColliders(tw);
                instantiatedTablewares.list.Add(container);
                woosh.pitch = Random.Range(0.8f, 1.2f);
                woosh.Play();

                //Remove this tableware from the player's hand.
                tablewareInHand.obj = null;
            }
        } else { //If the player is pointing nothing, hide the target
            target.SetActive(false);
        }
    }

    /// <summary>
    /// Return the table top surface under the specified target if existing ("under" meaning along the Vector3.down axis). If no table is under the target, return null.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private TableStruct? GetTableUnder(GameObject target) {

        TableStruct? res = null;
        RaycastHit[] hits = Physics.RaycastAll(target.transform.position, Vector3.down);
        int i = 0;
        while (res == null && i < hits.Length) { //For each object the ray collided with
            GameObject objectToAssess = hits[i].collider.gameObject;
            int j = 0;
            while (res == null && j < tableList.list.Count) {//for each table SO
                TableSO tableSO = ((TableSO)tableList.list[j]);
                int k = 0;
                while (res == null && k < tableSO.instances.Count) {//for each table instance of the current table SO
                    GameObject tableInstance = tableSO.instances[k];
                    if (tableInstance != null) {
                        if (tableInstance.GetComponent<TableHandler>().topSurface == objectToAssess) {
                            TableStruct tableStruct = new TableStruct();
                            tableStruct.reference = tableSO;
                            tableStruct.instance = tableInstance;
                            res = tableStruct;
                        }
                    }
                    k++;
                }
                j++;
            }
            i++;
        }
        return res;
    }

    private Vector3? GetAnchorWithNearestCompass(TableStruct tableStruct, GameObject target) {

        Vector3? res = null;

        TableSO.OrientationBinder[] orientationBinders = tableStruct.reference.orientationBinders;
        float minDist = float.MaxValue;
        for (int i = 0; i < orientationBinders.Length; i++) {
            TableSO.OrientationBinder binder = orientationBinders[i];
            TablewareZoneSO[] compasses = binder.compasses;
            for (int j = 0; j < compasses.Length; j++) {
                Vector3 zonePosition = Utils.GetVectorInLocalSpace(compasses[j].zoneLocalPosition, tableStruct.instance.transform) + tableStruct.instance.transform.position;
                float dist = (target.transform.position - zonePosition).sqrMagnitude;
                if (dist < minDist) {
                    minDist = dist;
                    res = Utils.GetVectorInLocalSpace(binder.anchor, tableStruct.instance.transform) + tableStruct.instance.transform.position;
                }
            }
        }

        return res;
    }

    private IEnumerator ThrowObjectWithBezierCoroutine(GameObject obj, Vector3 targetPoint) {

        Utils.EnablePhysics(obj, false);//Maybe useless? Physics are already disabled at the instantiation of the tableware in hand.
        float t = 0;
        Vector3 initPoint = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
        Vector3 p1 = initPoint + ((targetPoint - initPoint) / 2f) + new Vector3(0, 2, 0);
        while (t < throwingDuration) {
            if (obj == null) {
                //This can happen if the player resets the game while this object is being thrown. If so, end the coroutine now.
                yield break;
            }
            float ratio = t / throwingDuration;
            obj.transform.position = CalculateQuadraticBezierPoint(ratio, initPoint, p1, targetPoint);
            yield return new WaitForSeconds(Time.deltaTime);
            t += Time.deltaTime;
        }
        if (obj != null) {
            //This can happen if the player resets the game while this object is being thrown. If so, end the coroutine now.
            obj.transform.position = CalculateQuadraticBezierPoint(1, initPoint, p1, targetPoint);
            Utils.EnablePhysics(obj, true);
        }
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2) {
        //B(t) = (1-t)^2P0 + 2(t-1)tP1 + t^2P2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }

    private List<GameObject> GetChildrenWithColliders(GameObject obj) {
        List<GameObject> res = new List<GameObject>();
        if (obj.GetComponent<Collider>() != null && !obj.GetComponent<Collider>().isTrigger) {
            res.Add(obj);
        }
        foreach (Collider collider in obj.GetComponentsInChildren<Collider>()) {
            if (!collider.isTrigger) {
                res.Add(collider.gameObject);
            }
        }
        return res;
    }
}
