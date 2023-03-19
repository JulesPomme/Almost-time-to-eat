using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTimer {

    private float startTime;

    public ToolTimer() {
        startTime = -1;
    }

    public void Start(float time) {
        startTime = time;
    }

    public bool IsStarted() {
        return startTime != -1;
    }

    public void Stop() {
        startTime = -1;
    }

    public float GetElapsedTime(float time) {
        if (startTime == -1)
            return -1;
        return time - startTime;
    }
}
