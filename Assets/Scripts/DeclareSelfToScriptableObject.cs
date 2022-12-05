using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeclareSelfToScriptableObject : MonoBehaviour
{
    public enum ScriptableObjectType
    {
        GAMEOBJECT, POSITION, ROTATION, SCALE
    }
    public ScriptableObjectType type;
    public GameObjectSO gameObjectSO;
    public Vector3SO vector3SO;

    void Awake() {
        switch (type) {
            case ScriptableObjectType.GAMEOBJECT:
                gameObjectSO.obj = gameObject;
                break;
            case ScriptableObjectType.POSITION:
                vector3SO.value = transform.position;
                break;
            case ScriptableObjectType.ROTATION:
                vector3SO.value = transform.eulerAngles;
                break;
            case ScriptableObjectType.SCALE:
                vector3SO.value = transform.localScale;
                break;
            default:
                break;
        }
    }
}
