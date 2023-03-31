using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableEventHandler : MonoBehaviour {
    public GameObject topSurface;
    public TableSO table;
    public TablewareInstanceListSO tablewareRegister;
    public EventRegisterSO eventRegister;
    public ObservationSO resetObservation;
    public GameObject alertBubble;
    public IntegerSO nbCompletedTables;
    public RectTransform bubbleSlider;
    public AudioClip[] alertSounds;

    private List<GameObject> zones;
    private int resetId;
    private float minSliderBoundary = -85;
    private float maxSliderBoundary = -13;
    private int maxZoneCount;
    private List<AnimatorObserverSO> tablewareAnimatorObservers;
    private bool listeningToRemovingAnim;

    private void Awake() {
        table.instances.Clear();
    }

    private void Start() {
        resetId = resetObservation.AddObserver();
        zones = new List<GameObject>();
        tablewareAnimatorObservers = new List<AnimatorObserverSO>();
        table.instances.Add(gameObject);
        listeningToRemovingAnim = false;
    }

    private void Update() {

        //At receveiving a new event, start by removing potential old tablewares
        if (eventRegister.GetSettingTableEventProgress(gameObject) == EventRegisterSO.PROGRESS_INIT) {
            listeningToRemovingAnim = true;
            RemoveOldTablewareAnim();
            eventRegister.SetSettingTableEventProgress(gameObject, EventRegisterSO.PROGRESS_WIP_START);
        }
        //If old tableware removing animations are complete, complete the event process, destroy all the old tableware, init the bubbleSlider, etc.
        if (listeningToRemovingAnim && RemoveOldTablewareAnimIsDone()) {
            listeningToRemovingAnim = false;
            ClearTable();
            LaunchSetTableEvent();
        }

        //During an event, update the progress, eventually handle at finishing.
        if (EventRegisterSO.IS_PROGRESS_WIP(eventRegister.GetSettingTableEventProgress(gameObject))) {
            UpdateSetTableEventProgress();
        }

        //If reset is invoked
        if (resetObservation.CheckWarning(resetId)) {
            //Remove existing events or existing tablewares
            eventRegister.RemoveSetTableEvent(gameObject);
            alertBubble.GetComponent<Animator>().SetBool("deactivate", true);
            alertBubble.GetComponent<Animator>().Play("None");
            ClearTable();
            //Aknowledge reset information has been received
            resetObservation.AcknowledgeWarning(resetId);
        }
    }

    private void RemoveOldTablewareAnim() {
        //isResettingTable = true;
        foreach (GameObject tableware in tablewareRegister.GetTablewareInstances(gameObject)) {
            tableware.GetComponent<Animator>().SetTrigger("disappear");
            tablewareAnimatorObservers.Add(tableware.GetComponent<AnimatorObserver>().observer);
        }
    }

    private bool RemoveOldTablewareAnimIsDone() {

        bool allFinished = true;
        for (int i = tablewareAnimatorObservers.Count - 1; i >= 0; i--) {
            AnimatorObserverSO observer = tablewareAnimatorObservers[i];
            bool thisAnimIsFinished = observer.GetState("Disappear") == AnimatorObserverSO.State.FINISHED;
            allFinished &= thisAnimIsFinished;
            if (thisAnimIsFinished) {
                tablewareAnimatorObservers.RemoveAt(i);
            }
        }

        return allFinished;
    }

    private void ClearTable() {
        ClearZones();
        tablewareRegister.ClearOwner(gameObject);
        tablewareAnimatorObservers.Clear();

    }

    private void LaunchSetTableEvent() {
        InstantiateZones();
        bubbleSlider.localPosition = new Vector3(bubbleSlider.localPosition.x, minSliderBoundary, bubbleSlider.localPosition.z);
        alertBubble.GetComponent<Animator>().SetBool("deactivate", false);
        AudioSource alertAudio = alertBubble.GetComponent<AudioSource>();
        alertAudio.clip = alertSounds[Random.Range(0, alertSounds.Length)];
        alertAudio.pitch = Random.Range(0.8f, 1.2f);
        alertAudio.Play();
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

    private void UpdateSetTableEventProgress() {
        for (int i = zones.Count - 1; i >= 0; i--) {
            if (zones[i] == null) {
                zones.RemoveAt(i);
                float completionPercent = (maxZoneCount - zones.Count) / (float) maxZoneCount;
                eventRegister.SetSettingTableEventProgress(gameObject, completionPercent);
                float bubbleSliderY = minSliderBoundary + ((maxSliderBoundary - minSliderBoundary) * completionPercent);
                bubbleSlider.localPosition = new Vector3(bubbleSlider.localPosition.x, bubbleSliderY, bubbleSlider.localPosition.z);
                if (zones.Count == 0) {
                    alertBubble.GetComponent<Animator>().SetBool("deactivate", true);
                    GetComponent<AudioSource>().Play();
                    nbCompletedTables.value++;
                    eventRegister.RemoveSetTableEvent(gameObject);
                }
            }
        }
    }

    private void ClearZones() {
        while (zones.Count > 0) {
            Destroy(zones[zones.Count - 1]);
            zones.RemoveAt(zones.Count - 1);
        }
    }
}
