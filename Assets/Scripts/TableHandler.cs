using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableHandler : MonoBehaviour {
    public GameObject topSurface;
    public TableSO table;
    public ObservationSO resetObservation;
    public ScriptableObjectListSO tableList;
    public GameObject alertBubble;
    public IntegerSO nbCompletedTables;
    public RectTransform bubbleSlider;

    private List<GameObject> zones;
    private int resetId;
    private float minSliderBoundary = -85;
    private float maxSliderBoundary = -13;
    private int maxZoneCount;

    private void Awake() {
        table.instances.Clear();
        tableList.list.Clear();
    }

    private void Start() {
        resetId = resetObservation.AddObserver();
        zones = new List<GameObject>();
        table.instances.Add(gameObject);
        if (!tableList.list.Contains(table))
            tableList.list.Add(table);
        InstantiateZones();
    }

    private void InstantiateZones() {
        ScriptableObjectListSO zoneList = table.waveList;
        for (int j = 0; j < zoneList.list.Count; j++) {
            TablewareZoneSO zoneSO = ((TablewareZoneSO)zoneList.list[j]);
            GameObject zone = Instantiate(zoneSO.zonePrefab);
            zone.GetComponentInChildren<SetGhostTableware>().ghostPrefab = zoneSO.tableware.ghostPrefab;
            zone.GetComponentInChildren<SetGhostTableware>().table = gameObject;
            zone.GetComponentInChildren<UpdateScore>().myTableware = zoneSO.tableware;
            zones.Add(zone);
            zone.transform.parent = transform;
            zone.transform.localPosition = new Vector3(zoneSO.zoneLocalPosition.x, zoneSO.zoneLocalPosition.y, zoneSO.zoneLocalPosition.z);
        }
        maxZoneCount = zones.Count;
    }

    private void Update() {

        //If all tableware of,the table have been set
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
            while (zones.Count > 0) {
                Destroy(zones[zones.Count - 1]);
                zones.RemoveAt(zones.Count - 1);
            }
            InstantiateZones();
            resetObservation.AcknowledgeWarning(resetId);
            bubbleSlider.localPosition = new Vector3(bubbleSlider.localPosition.x, minSliderBoundary, bubbleSlider.localPosition.z);
            alertBubble.GetComponent<Animator>().SetBool("deactivate", false);
        }
    }
}
