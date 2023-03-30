using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Tableware Instance List")]
public class TablewareInstanceListSO : ScriptableObject {
    [System.Serializable]
    public struct Container {
        public GameObject instance;
        public TablewareSO reference;
        public List<Collider> physicalColliders;
    }

    private Dictionary<GameObject, List<Container>> registerPerTable = new Dictionary<GameObject, List<Container>>();
    private GameObject defaultOwner;

    public int Count() {
        int count = 0;
        foreach (GameObject table in registerPerTable.Keys) {
            count += registerPerTable[table].Count;
        }
        return count;
    }

    public void Add(GameObject owner, Container container) {

        if (!registerPerTable.ContainsKey(owner)) {
            registerPerTable[owner] = new List<Container>();
        }
        registerPerTable[owner].Add(container);
    }

    public void Clear() {
        foreach (GameObject owner in registerPerTable.Keys) {
            for (int i = 0; i < registerPerTable[owner].Count; i++) {
                Destroy(registerPerTable[owner][i].instance);
            }
        }
        registerPerTable.Clear();
    }

    public void ClearOwner(GameObject owner) {

        if (registerPerTable.ContainsKey(owner)) {
            for (int i = 0; i < registerPerTable[owner].Count; i++) {
                Destroy(registerPerTable[owner][i].instance);
            }
            registerPerTable.Remove(owner);
        }
    }

    public void ClearDefaultOwner() {
        ClearOwner(defaultOwner);
    }

    public bool Remove(GameObject instance) {

        bool res = false;
        int i = 0;
        List<GameObject> keys = new List<GameObject>(registerPerTable.Keys);
        while (!res && i < keys.Count) {
            int j = 0;
            while (!res && j < registerPerTable[keys[i]].Count) {
                if (registerPerTable[keys[i]][j].instance == instance) {
                    res = registerPerTable[keys[i]].Remove(registerPerTable[keys[i]][j]);
                }
                j++;
            }
            i++;
        }
        return res;
    }

    public Container? FindContainerWithCollider(Collider collider) {

        Container? res = null;
        int i = 0;
        List<GameObject> keys = new List<GameObject>(registerPerTable.Keys);
        while (!res.HasValue && i < keys.Count) {
            int j = 0;
            while (!res.HasValue && j < registerPerTable[keys[i]].Count) {
                List<Collider> objectsWithColliders = registerPerTable[keys[i]][j].physicalColliders;
                if (objectsWithColliders != null && objectsWithColliders.Contains(collider)) {
                    res = registerPerTable[keys[i]][j];
                }
                j++;
            }
            i++;
        }
        return res;
    }

    public GameObject FindOwner(GameObject instance) {

        GameObject owner = null;
        int i = 0;
        List<GameObject> keys = new List<GameObject>(registerPerTable.Keys);
        while (owner == null && i < keys.Count) {
            int j = 0;
            while (owner == null && j < registerPerTable[keys[i]].Count) {
                if (registerPerTable[keys[i]][j].instance == instance) {
                    owner = keys[i];
                }
                j++;
            }
            i++;
        }
        return owner;
    }

    public void SetDefaultOwner(GameObject g) {
        defaultOwner = g;
    }

    public GameObject GetDefaultOwner() {
        return defaultOwner;
    }

    public List<GameObject> GetTablewareInstances(GameObject owner) {

        List<GameObject> res = new List<GameObject>();
        List<Container> containers = new List<Container>();
        if (registerPerTable.ContainsKey(owner)) {
            containers.AddRange(registerPerTable[owner]);
            foreach (Container c in containers) {
                res.Add(c.instance);
            }
        }
        return res;
    }

    public static List<Collider> GetNestedPhysicalColliders(GameObject instance) {
        List<Collider> res = new List<Collider>();
        if (instance.GetComponent<Collider>() != null && !instance.GetComponent<Collider>().isTrigger) {
            res.Add(instance.GetComponent<Collider>());
        }
        foreach (Collider collider in instance.GetComponentsInChildren<Collider>()) {
            if (!collider.isTrigger) {
                res.Add(collider);
            }
        }
        return res;
    }

}
