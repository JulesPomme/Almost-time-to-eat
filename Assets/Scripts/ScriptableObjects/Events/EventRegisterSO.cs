using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Events/Register")]
public class EventRegisterSO : ScriptableObject {

    public readonly static float PROGRESS_NONE = -2;
    public readonly static float PROGRESS_INIT = -1;
    public readonly static float PROGRESS_WIP_START = 0;
    public static bool IS_PROGRESS_WIP(float i) { return i >= 0; }

    Dictionary<GameObject, Dictionary<EventSO, float>> eventsPerTable = new Dictionary<GameObject, Dictionary<EventSO, float>>();

    [SerializeField] private EventSO setTableEvent;

    public void Clear() {
        eventsPerTable.Clear();
    }

    public void AddSetTableEvent(GameObject table) {
        if (!eventsPerTable.ContainsKey(table)) {
            eventsPerTable[table] = new Dictionary<EventSO, float>();
        }
        if (!eventsPerTable[table].ContainsKey(setTableEvent)) {
            eventsPerTable[table][setTableEvent] = PROGRESS_INIT;
        }
    }

    public bool RemoveSetTableEvent(GameObject table) {
        bool res = false;
        if (eventsPerTable.ContainsKey(table)) {
            res = eventsPerTable[table].Remove(setTableEvent);
        }
        return res;
    }

    public float GetSettingTableEventProgress(GameObject table) {
        float res = PROGRESS_NONE;
        if (eventsPerTable.ContainsKey(table) && eventsPerTable[table].ContainsKey(setTableEvent)) {
            res = eventsPerTable[table][setTableEvent];
        }
        return res;
    }

    public void SetSettingTableEventProgress(GameObject table, float value) {
        if (eventsPerTable.ContainsKey(table)) {
            eventsPerTable[table][setTableEvent] = value;
        } else {
            throw new ArgumentNullException("No SetTableEvent found for table " + table.name + ". Can't set status to WIP. Create the event with the 'AddSetTableEvent' method first.");
        }
    }

    public int HowManyEventsRightNow() {
        int count = 0;
        foreach (Dictionary<EventSO, float> events in eventsPerTable.Values) {
            count += events.Count;
        }
        return count;
    }
}
