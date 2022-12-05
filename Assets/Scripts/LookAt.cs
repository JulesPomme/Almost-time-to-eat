using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    void Update()
    {
        Vector3 eulerAngles = transform.eulerAngles;
        transform.LookAt(Camera.main.transform);
        transform.eulerAngles = new Vector3(eulerAngles.x, transform.eulerAngles.y, eulerAngles.z);
    }
}
