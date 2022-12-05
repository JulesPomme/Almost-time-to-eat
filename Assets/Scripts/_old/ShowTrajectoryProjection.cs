using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowTrajectoryProjection : MonoBehaviour
{
    public GameObjectSO table;
    public GameObjectSO ground;
    public GameObjectListSO instantiatedTableware;
    public TablewareZoneSO currentWave;
    public FloatSO throwingForce;
    public Vector3SO throwingDirection;
    public LineRenderer lineRenderer;
    public int maxPhysicsFrameIterations = 100;
    public FloatSO minThrowingForce;
    public FloatSO maxThrowingForce;
    public GameObject crosshairPrefab;
    public int roundingDecimalsForCrosshair = 2;
    public int roundingDecimalsForGhosts = 2;
    public bool roundValues = true;

    private Scene simulationScene;
    private PhysicsScene physicsScene;

    //Table that relates ghost objects to their equivalent in the real scene
    private Dictionary<GameObject, GameObject> realToSimulationTable;
    private GameObject crosshairInstance;

    //DEBUG
    //private GameObject[] trajectoryObjects;
    //private bool hideTrajectoryObjects = false;
    //private GameObject previousThrowingObject;

    private void Start() {
        realToSimulationTable = new Dictionary<GameObject, GameObject>();
        crosshairInstance = Instantiate<GameObject>(crosshairPrefab);
        crosshairInstance.transform.SetParent(transform);
        crosshairInstance.SetActive(false);
        CreatePhysicsScene();
        //trajectoryObjects = new GameObject[maxPhysicsFrameIterations];
    }

    void Update() {
        //if (Input.GetKeyDown(KeyCode.H)) { hideTrajectoryObjects = !hideTrajectoryObjects; }
        //if (currentWave.tableware.obj != previousThrowingObject) {
        //    for (int i = 0; i < maxPhysicsFrameIterations; i++) {
        //        if (trajectoryObjects[i] != null)
        //            Destroy(trajectoryObjects[i]);
        //        //trajectoryObjects[i] = Instantiate<GameObject>(currentWave.tableware.obj.GetComponent<Ghost>().ghostPrefab);
        //        //DisablePhysics(trajectoryObjects[i]);
        //    }
        //    previousThrowingObject = currentWave.tableware.obj;
        //}

        UpdatePhysicsScene();
        if (throwingForce.value > minThrowingForce.value) {
            //lineRenderer.enabled = true;
            crosshairInstance.SetActive(true);
            SimulateTrajectory(currentWave.tableware.ghostPrefab, transform.position, throwingDirection.value, throwingForce.value);
        } else {
            //lineRenderer.enabled = false;
            crosshairInstance.SetActive(false);
        }
    }

    private void CreatePhysicsScene() {
        simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        physicsScene = simulationScene.GetPhysicsScene();

        UpdatePhysicsScene();
        ResetGhostObjects();
    }

    private void UpdatePhysicsScene() {

        //List all objects to instantiate in the simulation scene (basically: table + ground + tableware, the latest be it broken or not)
        List<GameObject> realSceneObjects = new List<GameObject>();
        realSceneObjects.Add(table.obj);
        realSceneObjects.Add(ground.obj);
        foreach (GameObject obj in instantiatedTableware.list) {
            realSceneObjects.Add(obj);
        }

        List<GameObject> real2SimKeys = new List<GameObject>(realToSimulationTable.Keys);
        List<GameObject> notInSimuYet = new List<GameObject>(realSceneObjects.Except(real2SimKeys));

        Scene mainScene = SceneManager.GetActiveScene();
        SceneManager.SetActiveScene(simulationScene);
        foreach (GameObject toAdd in notInSimuYet) {
            //### Attention ici code obsolète, le script Ghost n'existe plus. Pour que l'objet toAdd ait connaissance de son Ghost, il faut désormais faire appel au tableau des instantiatedTablewares...
            //GameObject ghost = Instantiate<GameObject>(toAdd.GetComponent<Ghost>().ghostPrefab);
            //ghost.name = toAdd.name;
            //DisableRenderers(ghost);
            //realToSimulationTable[toAdd] = ghost;
        }
        SceneManager.SetActiveScene(mainScene);

        foreach (GameObject key in real2SimKeys) {
            if (key == null) {//Removing objects that have been destroyed in the real scene
                Destroy(realToSimulationTable[key]);
                realToSimulationTable.Remove(key);
            }
        }
    }

    public void SimulateTrajectory(GameObject objectToThrow, Vector3 startPosition, Vector3 throwDirection, float throwingForce) {

        ResetGhostObjects();

        Scene mainScene = SceneManager.GetActiveScene();
        SceneManager.SetActiveScene(simulationScene);
        GameObject ghostObj = Instantiate<GameObject>(objectToThrow);
        SceneManager.SetActiveScene(mainScene);
        ghostObj.transform.position = startPosition;
        DisableRenderers(ghostObj);
        ghostObj.GetComponent<Rigidbody>().AddForce(throwDirection * throwingForce, ForceMode.Impulse);


        //DEBUG Print situation of all ghosts (transform + rigidbody) to see if something changes between two succesive iterations
        //Debug.Log("****************************************");
        //Debug.Log("BEFORE");
        //PrintMovingObjectStatus(ghostObj);
        //foreach (GameObject _obj in realToSimulationTable.Values) {
        //    PrintMovingObjectStatus(_obj);
        //}

        //lineRenderer.positionCount = maxPhysicsFrameIterations;
        for (int i = 0; i < maxPhysicsFrameIterations; i++) {
            physicsScene.Simulate(Time.fixedDeltaTime);
            if (roundValues) {
                RoundObjectValues(ghostObj, roundingDecimalsForCrosshair);
                RoundGhostValues(roundingDecimalsForGhosts);
            }
            //DEBUG Print again at each loop
            //Debug.Log("****************************************");
            //Debug.Log(i);
            //PrintMovingObjectStatus(ghostObj);
            //foreach (GameObject _obj in realToSimulationTable.Values) {
            //    PrintMovingObjectStatus(_obj);
            //}
            Vector3 pos = ghostObj.transform.position;
            pos.y += 0.01f;
            //lineRenderer.SetPosition(i, pos);
            //if (trajectoryObjects[i] != null) {
            //    if (!hideTrajectoryObjects) {
            //        trajectoryObjects[i].SetActive(true);
            //        trajectoryObjects[i].transform.position = pos;
            //        trajectoryObjects[i].transform.rotation = ghostObj.transform.rotation;
            //    } else {
            //        trajectoryObjects[i].SetActive(false);
            //    }
            //}
            if (i == maxPhysicsFrameIterations - 1) {
                crosshairInstance.transform.position = pos;
            }
        }
        Destroy(ghostObj);

        //DEBUG Print again at the end
        //Debug.Log("****************************************");
        //Debug.Log("AFTER");
        //PrintMovingObjectStatus(ghostObj);
        //foreach (GameObject _obj in realToSimulationTable.Values) {
        //    PrintMovingObjectStatus(_obj);
        //}

    }

    private void PrintMovingObjectStatus(GameObject obj) {
        string str = "";
        if (obj.GetComponent<Rigidbody>() != null && obj.GetComponent<Rigidbody>().velocity != Vector3.zero) {
            str = obj.name;
            str += ", pos = " + obj.transform.position + ", rot " + obj.transform.eulerAngles;
            str += ", vel = " + obj.GetComponent<Rigidbody>().velocity + ", angVel = " + obj.GetComponent<Rigidbody>().angularVelocity + ", magnVel = " + obj.GetComponent<Rigidbody>().velocity.magnitude;
            Debug.Log(str);
        }
        foreach (Transform child in obj.transform) {
            if (child.GetComponent<Rigidbody>() != null && child.GetComponent<Rigidbody>().velocity != Vector3.zero) {
                str = child.name;
                str += ", pos = " + child.position + ", rot " + child.eulerAngles;
                str += ", vel = " + child.GetComponent<Rigidbody>().velocity + ", angVel = " + child.GetComponent<Rigidbody>().angularVelocity + ", magnVel = " + child.GetComponent<Rigidbody>().velocity.magnitude;
                Debug.Log(str);
            }
        }
    }

    private void RoundGhostValues(int nbDecimals) {
        List<GameObject> real2SimKeys = new List<GameObject>(realToSimulationTable.Keys);
        foreach (GameObject key in real2SimKeys) {
            //Round transform and rigidbody values for all the ghosts on the table (in order to avoid floating discrepancies between two simulations)
            RoundObjectValues(realToSimulationTable[key], nbDecimals);
        }
    }

    private void RoundObjectValues(GameObject obj, int nbDecimals) {
        //Round transform and rigidbody values for all the ghosts on the table (in order to avoid floating discrepancies between two simulations)
        if (obj.GetComponent<Rigidbody>() != null) {
            Vector3 vel = obj.GetComponent<Rigidbody>().velocity;
            obj.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Round(vel.x * 100) * 0.01f, Mathf.Round(vel.y * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals), Mathf.Round(vel.z * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals));
            Vector3 angVel = obj.GetComponent<Rigidbody>().angularVelocity;
            obj.GetComponent<Rigidbody>().angularVelocity = new Vector3(Mathf.Round(angVel.x * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals), Mathf.Round(angVel.y * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals), Mathf.Round(angVel.z * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals));
        }
        foreach (Rigidbody rgbd in obj.GetComponentsInChildren<Rigidbody>()) {
            Vector3 vel = rgbd.velocity;
            rgbd.velocity = new Vector3(Mathf.Round(vel.x * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals), Mathf.Round(vel.y * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals), Mathf.Round(vel.z * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals));
            Vector3 angVel = rgbd.angularVelocity;
            rgbd.angularVelocity = new Vector3(Mathf.Round(angVel.x * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals), Mathf.Round(angVel.y * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals), Mathf.Round(angVel.z * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals));
        }
        Vector3 pos = obj.transform.position;
        obj.transform.position = new Vector3(Mathf.Round(pos.x * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals), Mathf.Round(pos.y * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals), Mathf.Round(pos.z * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals));
        Vector3 rot = obj.transform.eulerAngles;
        obj.transform.eulerAngles = new Vector3(Mathf.Round(rot.x * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals), Mathf.Round(rot.y * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals), Mathf.Round(rot.z * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals));
        foreach (Transform child in obj.GetComponentsInChildren<Transform>()) {
            pos = child.transform.position;
            child.transform.position = new Vector3(Mathf.Round(pos.x * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals), Mathf.Round(pos.y * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals), Mathf.Round(pos.z * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals));
            rot = child.transform.eulerAngles;
            child.transform.eulerAngles = new Vector3(Mathf.Round(rot.x * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals), Mathf.Round(rot.y * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals), Mathf.Round(rot.z * Mathf.Pow(10, nbDecimals)) * Mathf.Pow(10, -nbDecimals));
        }
    }

    private void ResetGhostObjects() {

        List<GameObject> real2SimKeys = new List<GameObject>(realToSimulationTable.Keys);
        foreach (GameObject key in real2SimKeys) {
            //Reset transform and rigidbody for all the ghosts on the table
            GameObject ghost = realToSimulationTable[key];
            ResetRigidBody(ghost, key);
            ResetTransform(ghost, key);
        }
    }

    private void ResetTransform(GameObject obj, GameObject refObj) {

        obj.transform.position = refObj.transform.position;
        obj.transform.rotation = refObj.transform.rotation;
        for (int i = 0; i < obj.transform.childCount; i++) {
            obj.transform.GetChild(i).transform.position = refObj.transform.GetChild(i).transform.position;
            obj.transform.GetChild(i).transform.rotation = refObj.transform.GetChild(i).transform.rotation;
        }
    }

    private void ResetRigidBody(GameObject obj, GameObject refObj) {
        //Debug.Log("****************************************");
        //Debug.Log("RESETTING");
        if (obj.GetComponent<Rigidbody>() != null) {
            //if (obj.GetComponent<Rigidbody>().velocity != Vector3.zero)
            //    Debug.Log(obj.name + " to pos " + refObj.transform.position + ", rot " + refObj.transform.eulerAngles + ", vel " + refObj.GetComponent<Rigidbody>().velocity + ", angVel = " + refObj.GetComponent<Rigidbody>().angularVelocity);
            obj.GetComponent<Rigidbody>().velocity = refObj.GetComponent<Rigidbody>().velocity;
            obj.GetComponent<Rigidbody>().angularVelocity = refObj.GetComponent<Rigidbody>().angularVelocity;
        }
        for (int i = 0; i < obj.transform.childCount; i++) {
            if (obj.transform.GetChild(i).GetComponent<Rigidbody>() != null) {
                //if (obj.transform.GetChild(i).GetComponent<Rigidbody>().velocity != Vector3.zero)
                //    Debug.Log(obj.transform.GetChild(i).name + " to pos " + refObj.transform.GetChild(i).transform.position + ", rot " + refObj.transform.GetChild(i).transform.eulerAngles + ", vel " + refObj.transform.GetChild(i).GetComponent<Rigidbody>().velocity + ", angVel = " + refObj.transform.GetChild(i).GetComponent<Rigidbody>().angularVelocity);
                obj.transform.GetChild(i).GetComponent<Rigidbody>().velocity = refObj.transform.GetChild(i).GetComponent<Rigidbody>().velocity;
                obj.transform.GetChild(i).GetComponent<Rigidbody>().angularVelocity = refObj.transform.GetChild(i).GetComponent<Rigidbody>().angularVelocity;
            }
        }
    }

    private void DisableRenderers(GameObject obj) {
        if (obj.GetComponent<Renderer>() != null)
            obj.GetComponent<Renderer>().enabled = false;
        foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>()) {
            renderer.enabled = false;
        }
    }

    private void DisablePhysics(GameObject obj) {
        if (obj.GetComponent<Collider>() != null)
            obj.GetComponent<Collider>().enabled = false;
        foreach (Collider collider in obj.GetComponentsInChildren<Collider>()) {
            collider.enabled = false;
        }

        if (obj.GetComponent<Rigidbody>() != null)
            obj.GetComponent<Rigidbody>().isKinematic = true;
        foreach (Rigidbody rgbd in obj.GetComponentsInChildren<Rigidbody>()) {
            rgbd.isKinematic = true;
        }
    }
}
