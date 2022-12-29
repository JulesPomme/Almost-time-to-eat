using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnchorCompassRotation : MonoBehaviour {

    public Transform anchor;
    public Transform compass;

    void Start() {

    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Vector3 compassPos = Utils.GetVectorInLocalSpace(compass.localPosition, compass.parent.transform) + compass.parent.transform.position;
            Debug.Log("compass Pos = " + compassPos);
            Vector3 anchorPos = anchor.position;
            Debug.Log("anchorPos = " + anchorPos);
            Debug.Log("diff vector between compass and fork = " + (compassPos - compass.transform.parent.transform.position));
            Debug.Log("diff vector between anchor and fork = " + (anchorPos - compass.transform.parent.transform.position));

            Quaternion rotationToAnchor = Quaternion.FromToRotation(compassPos - compass.transform.parent.transform.position, anchorPos - compass.transform.parent.transform.position);
            Debug.Log("rotation to anchor euler angles = " + rotationToAnchor.eulerAngles);
            compass.transform.parent.transform.rotation = rotationToAnchor;

            Debug.Log("compass right before = " + compass.right + " up = " + Vector3.up);
            Quaternion rotationToUpwards = Quaternion.FromToRotation(compass.right, Vector3.up);
            Debug.Log("rotation to upwards euler angles = " + rotationToUpwards.eulerAngles);
            compass.transform.parent.transform.Rotate(rotationToUpwards.eulerAngles);
            Debug.Log("compass right after = " + compass.right);
        }
    }
}
