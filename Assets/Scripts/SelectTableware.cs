using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTableware : MonoBehaviour
{
    public ScriptableObjectListSO availableTablewares;

    void Start() {
        availableTablewares.cursor = 0;
    }

    void Update() {

        float scrollValue = Input.mouseScrollDelta.y;
        bool update = false;
        if (scrollValue != 0) {
            availableTablewares.cursor += (int)scrollValue;
            update = true;
        } else if (Input.GetKeyDown(KeyCode.LeftShift)) {
            availableTablewares.cursor++;
            update = true;
        } else if (Input.GetKeyDown(KeyCode.LeftControl)) {
            availableTablewares.cursor--;
            update = true;
        }

        if (update) {
            if (availableTablewares.cursor >= availableTablewares.list.Count) {
                availableTablewares.cursor = availableTablewares.cursor % availableTablewares.list.Count;
            } else if (availableTablewares.cursor < 0) {
                availableTablewares.cursor = availableTablewares.list.Count + availableTablewares.cursor;
            }
        }
    }
}
