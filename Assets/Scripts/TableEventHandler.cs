using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableEventHandler : MonoBehaviour {

    public TableSO table;
    public TablewareInstanceListSO tablewareRegister;
    public EventRegisterSO eventRegister;
    public ObservationSO resetObservation;
    public IntegerSO nbCompletedTables;
    public AnimatorObserverSO animatorObserver;


    public GameObject topSurface;
    public GameObject alertBubble;
    public RectTransform bubbleSlider;
    public Image bubbleTimer;
    public AudioClip[] alertSounds;

    private List<GameObject> zones;
    private int resetId;
    private float minSliderBoundary = -91;
    private float maxSliderBoundary = -10;
    private int maxZoneCount;
    private bool listeningToRemovingAnimAfterInit;
    private bool listeningToRemovingAnimAfterTimeUp;
    private ToolTimer eventTimer;

    private void Awake() {
        table.instances.Clear();
    }

    private void Start() {
        resetId = resetObservation.AddObserver();
        zones = new List<GameObject>();
        table.instances.Add(gameObject);
        listeningToRemovingAnimAfterInit = false;
        listeningToRemovingAnimAfterTimeUp = false;
        eventTimer = new ToolTimer();
    }

    private void Update() {

        //At receveiving a new event, start by removing potential old tablewares
        if (eventRegister.GetSettingTableEventProgress(gameObject) == EventRegisterSO.PROGRESS_INIT) {
            listeningToRemovingAnimAfterInit = true;
            eventRegister.SetSettingTableEventProgress(gameObject, EventRegisterSO.PROGRESS_WIP_START);
            eventTimer.Start(Time.time);
            bubbleTimer.fillAmount = 0;
        }

        //If old tableware removing animations are complete, complete the event process, destroy all the old tableware, init the bubbleSlider, etc.
        if (listeningToRemovingAnimAfterInit && IsRemovingAnimationDone()) {
            listeningToRemovingAnimAfterInit = false;
            LaunchSetTableEvent();
        }

        //If time is up, removing animation has been called, we must now reset the table.
        if (listeningToRemovingAnimAfterTimeUp && IsRemovingAnimationDone()) {
            listeningToRemovingAnimAfterTimeUp = false;
            eventRegister.RemoveSetTableEvent(gameObject);
            alertBubble.GetComponent<Animator>().SetBool("appear", false);
            alertBubble.GetComponent<Animator>().SetBool("win", false);
        }

        //During an event, update the progress, eventually handle at finishing.
        if (EventRegisterSO.IS_PROGRESS_WIP(eventRegister.GetSettingTableEventProgress(gameObject))) {
            UpdateSetTableEventProgress();
        }

        //If reset is invoked
        if (resetObservation.CheckWarning(resetId)) {
            //Remove existing events or existing tablewares
            eventRegister.RemoveSetTableEvent(gameObject);
            alertBubble.GetComponent<Animator>().SetBool("appear", false);
            alertBubble.GetComponent<Animator>().SetBool("win", false);
            StopObservingAnimators();
            ClearTable();
            //Aknowledge reset information has been received
            resetObservation.AcknowledgeWarning(resetId);
        }
    }

    private bool IsRemovingAnimationDone() {

        bool allFinished = true;

        //TABLEWARES
        List<GameObject> _tablewares = new List<GameObject>(tablewareRegister.GetTablewareInstances(gameObject));
        foreach (GameObject tableware in _tablewares) {
            AnimatorObserverSO.State animState = animatorObserver.GetState(tableware, "Disappear");
            bool thisAnimIsFinished = animState == AnimatorObserverSO.State.FINISHED;
            allFinished &= thisAnimIsFinished;
            if (thisAnimIsFinished) {
                animatorObserver.StopObserving(tableware);
                tablewareRegister.DestroyTableware(tableware);
            } else if (animState == AnimatorObserverSO.State.NONE) {//Start the removing animation if it's not done yet
                tableware.GetComponent<Animator>().SetTrigger("disappear");
            }
        }

        //ZONES
        List<GameObject> _zones = new List<GameObject>(zones);
        foreach (GameObject zone in _zones) {
            if (zone == null)//FIXME: should not call this, zone managing should be rethought
                continue;
            foreach (Animator zoneAnimator in zone.GetComponentsInChildren<Animator>()) {
                AnimatorObserverSO.State animState = animatorObserver.GetState(zoneAnimator.gameObject, "Disappear");
                bool thisAnimIsFinished = animState == AnimatorObserverSO.State.FINISHED;
                allFinished &= thisAnimIsFinished;
                if (thisAnimIsFinished) {
                    animatorObserver.StopObserving(zoneAnimator.gameObject);
                    zones.Remove(zone);
                    Destroy(zone);
                } else if (animState == AnimatorObserverSO.State.NONE) {//Start the removing animation if it's not done yet
                    zoneAnimator.SetBool("disappear", true);
                }
            }
        }

        return allFinished;
    }

    private void StopObservingAnimators() {
        foreach (GameObject tableware in tablewareRegister.GetTablewareInstances(gameObject)) {
            animatorObserver.StopObserving(tableware);
        }
        foreach (GameObject zone in zones) {
            foreach (Animator zoneAnimator in zone.GetComponentsInChildren<Animator>()) {
                animatorObserver.StopObserving(zoneAnimator.gameObject);
            }
        }
    }

    private void ClearTable() {
        ClearZones();
        tablewareRegister.ClearOwner(gameObject);
    }

    private void LaunchSetTableEvent() {
        InstantiateZones();
        bubbleSlider.localPosition = new Vector3(bubbleSlider.localPosition.x, minSliderBoundary, bubbleSlider.localPosition.z);
        alertBubble.GetComponent<Animator>().SetBool("win", false);
        alertBubble.GetComponent<Animator>().SetBool("appear", true);
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

        //If one or more zones are on the table, loop
        for (int i = zones.Count - 1; i >= 0; i--) {
            //This zone has been validated recently, remove the object, update the bubble
            if (zones[i] == null) {
                zones.RemoveAt(i);
                float completionPercent = (maxZoneCount - zones.Count) / (float)maxZoneCount;
                eventRegister.SetSettingTableEventProgress(gameObject, completionPercent);
                float bubbleSliderY = minSliderBoundary + ((maxSliderBoundary - minSliderBoundary) * completionPercent);
                bubbleSlider.localPosition = new Vector3(bubbleSlider.localPosition.x, bubbleSliderY, bubbleSlider.localPosition.z);
                //If this was the last zone, trigger the win animation and sound
                if (zones.Count == 0) {
                    alertBubble.GetComponent<Animator>().SetBool("appear", false);
                    alertBubble.GetComponent<Animator>().SetBool("win", true);
                    GetComponent<AudioSource>().Play();
                    nbCompletedTables.value++;
                    eventRegister.RemoveSetTableEvent(gameObject);
                }
            }
        }

        //Check the event timer, cancel event if time is up
        float elapsedTimePercent = eventTimer.GetElapsedTime(Time.time) / (float)table.settingTableTimer;
        if (elapsedTimePercent >= 1) {
            listeningToRemovingAnimAfterTimeUp = true;
            eventTimer.Stop();
        } else if (elapsedTimePercent >= 0) { // when no event is registered, the elapsed time is negative => no need to update.
            bubbleTimer.fillAmount = elapsedTimePercent;
        }
    }

    private void ClearZones() {
        while (zones.Count > 0) {
            Destroy(zones[zones.Count - 1]);
            zones.RemoveAt(zones.Count - 1);
        }
    }
}
