using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGhostTableware : MonoBehaviour {
    public GameObject ghostPrefab;
    public Transform basePosition;
    public GameObject table;

    void Start() {
        GameObject ghost = Instantiate<GameObject>(ghostPrefab);
        ghost.transform.parent = transform;
        ghost.transform.localPosition = new Vector3(0, basePosition.localPosition.y + 0.01f, 0);
        float orientation = ghost.transform.eulerAngles.y + Vector3.SignedAngle(Vector3.right, ghost.transform.position - table.transform.position, Vector3.up);
        ghost.transform.eulerAngles = new Vector3(ghost.transform.eulerAngles.x, orientation, ghost.transform.eulerAngles.z);
    }
}
