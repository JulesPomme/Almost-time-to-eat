using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableEventScheduler : MonoBehaviour {
    public ObservationSO resetObservation;

    private TableHandler tableHandler;
    private ToolTimer timer;
    private int resetId;

    void Start() {
        resetId = resetObservation.AddObserver();
        tableHandler = GetComponent<TableHandler>();
        timer = new ToolTimer();
    }

    void Update() {

        if (resetObservation.CheckWarning(resetId)) {
            timer.Stop();
            resetObservation.AcknowledgeWarning(resetId);
        }

        if (tableHandler.IsCompleted() && !tableHandler.IsResettingTable() && !timer.IsStarted()) {
            timer.Start(Time.time);
        }

        if (timer.IsStarted() && timer.GetElapsedTime(Time.time) >= tableHandler.table.howLongToRefreshInSeconds) {
            tableHandler.InitiateResetTable(true);
            timer.Stop();
        }
    }
}
