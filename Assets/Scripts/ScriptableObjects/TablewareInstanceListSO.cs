using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Tableware Instance List")]
public class TablewareInstanceListSO : ScriptableObject
{
    [System.Serializable]
    public struct Container
    {
        public GameObject instance;
        public TablewareSO reference;
        public List<GameObject> objectsWithColliders;
    }

    public List<Container> list;

    public bool Remove(GameObject instance) {

        bool res = false;
        bool found = false;
        int i = 0;
        while (!found && i < list.Count) {
            if (list[i].instance == instance) {
                res = list.Remove(list[i]);
                found = true;
            }
            i++;
        }
        return res;
    }
    public bool ContainsCollider(GameObject obj) {

        bool found = false;
        int i = 0;
        while (!found && i < list.Count) {
            if (list[i].objectsWithColliders != null && list[i].objectsWithColliders.Contains(obj)) {
                found = true;
            }
            i++;
        }
        return found;
    }

    public Container? GetContainerWithCollider(GameObject objCollider) {

        Container? res = null;
        bool found = false;
        int i = 0;
        while (!found && i < list.Count) {
            if (list[i].objectsWithColliders != null && list[i].objectsWithColliders.Contains(objCollider)) {
                found = true;
                res = list[i];
            }
            i++;
        }
        return res;
    }
}
