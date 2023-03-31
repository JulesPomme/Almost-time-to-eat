using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventScheduler : MonoBehaviour {

    public EventRegisterSO eventRegister;
    public ObservationSO resetObservation;
    public ScriptableObjectListSO tableList;

    private ToolTimer timer;
    private int resetId;
    private List<GameObject> tableInstances;
    private GameObject previousTable;

    void Start() {
        resetId = resetObservation.AddObserver();
        timer = new ToolTimer();
        timer.Start(Time.time);
        tableInstances = new List<GameObject>();
        foreach (TableSO tableSO in tableList.list) {
            tableInstances.AddRange(tableSO.instances);
        }
    }

    void Update() {

        if (resetObservation.CheckWarning(resetId)) {
            resetObservation.AcknowledgeWarning(resetId);
            timer.Restart(Time.time);
        }

        if (timer.GetElapsedTime(Time.time) >= 1) {//Possible event every second
            timer.Restart(Time.time);
            int proba = Random.Range(0, 101);
            Debug.Log(proba);
            bool triggerEvent = false;
            switch (eventRegister.HowManyEventsRightNow()) {
                case 0:
                    triggerEvent = proba >= 0;
                    break;
                case 1:
                    triggerEvent = proba > 95; //5% chance to trigger an event every second, i.e., on average, 5 triggers every 100 seconds, i.e., 1 trigger every 20 second.
                    break;
                //case 2:
                //    triggerEvent = proba > 99;
                //    break;
                default:
                    break;
            }

            List<GameObject> availableTables = GetAvailableTablesForSetEvent();
            availableTables.Remove(previousTable);
            if (triggerEvent && availableTables.Count > 0) {
                int index = Random.Range(0, availableTables.Count);
                eventRegister.AddSetTableEvent(availableTables[index]);
                previousTable = availableTables[index];
            }
        }
    }

    private List<GameObject> GetAvailableTablesForSetEvent() {
        List<GameObject> res = new List<GameObject>();
        foreach (GameObject table in tableInstances) {
            if (eventRegister.GetSettingTableEventStatus(table) == EventRegisterSO.EventStatus.NONE) {
                res.Add(table);
            }
        }
        return res;
    }
}
