using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableHandler : MonoBehaviour {
    public GameObject topSurface;
    public TableSO table;
    public TablewareInstanceListSO tablewareRegister;
    public ObservationSO resetObservation;
    public GameObject alertBubble;
    public IntegerSO nbCompletedTables;
    public RectTransform bubbleSlider;

    private List<GameObject> zones;
    private int resetId;
    private float minSliderBoundary = -85;
    private float maxSliderBoundary = -13;
    private int maxZoneCount;
    private List<AnimatorObserverSO> tablewareAnimatorObservers;
    private bool isResettingTable;

    private void Awake() {
        table.instances.Clear();
    }

    private void Start() {
        resetId = resetObservation.AddObserver();
        zones = new List<GameObject>();
        tablewareAnimatorObservers = new List<AnimatorObserverSO>();
        isResettingTable = false;
        table.instances.Add(gameObject);
        InstantiateZones();
    }

    private void InstantiateZones() {
        ScriptableObjectListSO zoneList = table.waveList;
        for (int j = 0; j < zoneList.list.Count; j++) {
            TablewareZoneSO zoneSO = ((TablewareZoneSO)zoneList.list[j]);
            GameObject zone = Instantiate(zoneSO.zonePrefab);
            SetGhostTableware ghostScript = zone.GetComponentInChildren<SetGhostTableware>();
            ghostScript.SetTableSO(table);
            ghostScript.SetZoneSO(zoneSO);
            ghostScript.SetTableInstanceTransform(transform);
            zone.GetComponentInChildren<UpdateScore>().myTableware = zoneSO.tableware;
            zones.Add(zone);
            zone.transform.parent = transform;
            zone.transform.localPosition = new Vector3(zoneSO.zoneLocalPosition.x, zoneSO.zoneLocalPosition.y, zoneSO.zoneLocalPosition.z);
        }
        maxZoneCount = zones.Count;
    }

    private void Update() {

        //If all tablewares of the table have been set
        for (int i = zones.Count - 1; i >= 0; i--) {
            if (zones[i] == null) {
                zones.RemoveAt(i);
                float bubbleSliderY = minSliderBoundary + ((maxSliderBoundary - minSliderBoundary) * (maxZoneCount - zones.Count) / maxZoneCount);
                bubbleSlider.localPosition = new Vector3(bubbleSlider.localPosition.x, bubbleSliderY, bubbleSlider.localPosition.z);
                if (zones.Count == 0) {
                    alertBubble.GetComponent<Animator>().SetBool("deactivate", true);
                    GetComponent<AudioSource>().Play();
                    nbCompletedTables.value++;
                }
            }
        }

        //If reset is invoked
        if (resetObservation.CheckWarning(resetId)) {
            //Initiate the reset animations
            InitiateResetTable();
            //Aknowledge reset information has been received
            resetObservation.AcknowledgeWarning(resetId);
        }

        //If reset animations are complete, complete the reset process, destroy all the tableware, reinit the bubbleSlider, etc.
        if (CheckTablewareAnimatorObservers()) {
            CompleteResetTable();
        }
    }
    public bool IsCompleted() {
        return zones.Count == 0;
    }

    /// <summary>
    /// Return true as soon as InitiateResetTable as been called, while CompleteResetTable has not been called yet.
    /// </summary>
    /// <returns></returns>
    public bool IsResettingTable() {
        return isResettingTable;
    }

    public void InitiateResetTable(bool withDecoration = false) {
        isResettingTable = true;
        if (withDecoration) {
            foreach (GameObject tableware in tablewareRegister.GetTablewareInstances(gameObject)) {
                tableware.GetComponent<Animator>().SetTrigger("disappear");
                tablewareAnimatorObservers.Add(tableware.GetComponent<AnimatorObserver>().observer);
            }
        } else {
            CompleteResetTable();
        }
    }

    private bool CheckTablewareAnimatorObservers() {

        bool allFinished = true;
        foreach (AnimatorObserverSO observer in tablewareAnimatorObservers) {
            allFinished &= observer.GetState("Disappear") == AnimatorObserverSO.State.FINISHED;
        }

        return tablewareAnimatorObservers.Count > 0 && allFinished;
    }

    private void CompleteResetTable() {
        while (zones.Count > 0) {
            Destroy(zones[zones.Count - 1]);
            zones.RemoveAt(zones.Count - 1);
        }
        InstantiateZones();
        bubbleSlider.localPosition = new Vector3(bubbleSlider.localPosition.x, minSliderBoundary, bubbleSlider.localPosition.z);
        //TODO : ajouter un son d'apparation de la bubble (un sifflement ? ou un "hep serveur" ?) + une animation un peu plus chiadée
        alertBubble.GetComponent<Animator>().SetBool("deactivate", false);

        tablewareRegister.ClearOwner(gameObject);
        tablewareAnimatorObservers.Clear();
        isResettingTable = false;
    }
}
