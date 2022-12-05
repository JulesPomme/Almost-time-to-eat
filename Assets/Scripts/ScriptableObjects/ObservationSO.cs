using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//An implementation of the pattern observer via Scriptable Objects
[CreateAssetMenu(menuName = "Scriptable Objects/Observation")]
public class ObservationSO : ScriptableObject {
    private List<int> observerIds;
    private int count;
    private List<bool> flags;

    public void StartObservation() {
        observerIds = new List<int>();
        count = 0;
        flags = new List<bool>();
    }

    public int AddObserver() {

        observerIds.Add(count);
        count++;
        flags.Add(false);
        return count - 1;
    }

    public void Warn() {
        for (int i = 0; i < flags.Count; i++) {
            flags[i] = true;
        }
    }

    public bool CheckWarning(int observerId) {
        return flags[observerId];
    }

    public void AcknowledgeWarning(int observerId) {
        flags[observerId] = false;
    }
}
