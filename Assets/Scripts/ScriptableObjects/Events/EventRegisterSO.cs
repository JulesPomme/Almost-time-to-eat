using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Events/Register")]
public class EventRegisterSO : ScriptableObject {

    public enum EventStatus {
        INIT, WIP, NONE
    }

    Dictionary<GameObject, Dictionary<EventSO, EventStatus>> eventsPerTable = new Dictionary<GameObject, Dictionary<EventSO, EventStatus>>();

    public EventSO setTableEvent;

    public void Clear() {
        eventsPerTable.Clear();
    }

    public void AddSetTableEvent(GameObject table) {
        if (!eventsPerTable.ContainsKey(table)) {
            eventsPerTable[table] = new Dictionary<EventSO, EventStatus>();
        }
        if (!eventsPerTable[table].ContainsKey(setTableEvent)) {
            eventsPerTable[table][setTableEvent] = EventStatus.INIT;
        }
    }

    public bool RemoveSetTableEvent(GameObject table) {
        bool res = false;
        if (eventsPerTable.ContainsKey(table)) {
            res = eventsPerTable[table].Remove(setTableEvent);
        }
        return res;
    }

    public EventStatus GetSettingTableEventStatus(GameObject table) {
        EventStatus res = EventStatus.NONE;
        if (eventsPerTable.ContainsKey(table) && eventsPerTable[table].ContainsKey(setTableEvent)) {
            res = eventsPerTable[table][setTableEvent];
        }
        return res;
    }

    public void SetSettingTableEventToWIP(GameObject table) {
        if (eventsPerTable.ContainsKey(table)) {
            eventsPerTable[table][setTableEvent] = EventStatus.WIP;
        } else {
            throw new ArgumentNullException("No SetTableEvent found for table " + table.name + ". Can't set status to WIP. Create the event with the 'AddSetTableEvent' method first.");
        }
    }

    public int HowManyEventsRightNow() {
        int count = 0;
        foreach (Dictionary<EventSO, EventStatus> events in eventsPerTable.Values) {
            count += events.Count;
        }
        return count;
    }

}
