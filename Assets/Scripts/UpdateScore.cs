using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateScore : MonoBehaviour
{

    public IntegerSO score;
    public GameObject basePosition;
    public int minScore;
    public int maxScore;
    public int nbSlices;
    public TablewareSO myTableware;
    public IntegerSO currentWaveRemainingTargets;
    public TablewareInstanceListSO instantiatedTablewares;
    public GameObject uiCanvas;
    public AudioSourceSO inGameVoices;
    public AudioClip[] goodScoreClips;
    public AudioClip[] mediumScoreClips;
    public AudioClip[] badScoreClips;
    public AudioSourceSO pickupAudio;

    private float zoneRadius;
    private Dictionary<GameObject, Vector3?> previousPositionDict;
    private List<GameObject> alreadyEnteredObjects;
    private bool destroying;

    void Start() {
        zoneRadius = transform.localScale.x / 2f;
        previousPositionDict = new Dictionary<GameObject, Vector3?>();
        alreadyEnteredObjects = new List<GameObject>();
        destroying = false;
    }

    private void OnTriggerEnter(Collider other) {
        TablewareInstanceListSO.Container? enteringTableware = instantiatedTablewares.GetContainerWithCollider(other.gameObject);
        if (enteringTableware.HasValue && enteringTableware.Value.reference == myTableware && !alreadyEnteredObjects.Contains(other.gameObject)) {
            previousPositionDict[other.gameObject] = null;
            alreadyEnteredObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other) {
        GameObject otherObj = other.gameObject;
        if (!destroying && previousPositionDict.ContainsKey(otherObj)) {
            if (previousPositionDict[otherObj] == otherObj.transform.position) {
                previousPositionDict.Remove(otherObj);
                float distance = (basePosition.transform.position - otherObj.transform.position).magnitude;
                distance = Mathf.Clamp(distance, 0, zoneRadius);
                float myScore = maxScore - ((maxScore - minScore) * distance / zoneRadius);
                float scoreRange = maxScore - minScore;
                float sliceRange = scoreRange / nbSlices;
                float slice = minScore;
                while (myScore > slice && slice < maxScore) {
                    slice += sliceRange;
                }
                int myScoreInt = slice > maxScore ? maxScore : (int)slice;
                score.value += myScoreInt;
                PlayVoice(myScore);
                pickupAudio.source.Play();
                currentWaveRemainingTargets.value--;
                DestroyAfterValidation(myScoreInt, currentWaveRemainingTargets.value == 0);
                destroying = true;
            } else {
                previousPositionDict[otherObj] = otherObj.transform.position;
            }
        }
    }

    private void PlayVoice(float myScore) {

        AudioClip selectedClip = null;
        if (myScore == minScore) {
            selectedClip = badScoreClips[Random.Range(0, badScoreClips.Length)];
        } else if (myScore == maxScore) {
            selectedClip = mediumScoreClips[Random.Range(0, mediumScoreClips.Length)];
        } else {
            selectedClip = goodScoreClips[Random.Range(0, goodScoreClips.Length)];
        }
        inGameVoices.source.Stop();
        inGameVoices.source.clip = selectedClip;
        inGameVoices.source.pitch = Random.Range(0.95f, 1.1f);
        inGameVoices.source.Play();
    }

    private void DestroyAfterValidation(int myScore, bool destroyParent) {
        StartCoroutine(DisplayScoreCoroutine(uiCanvas, myScore, destroyParent));
    }

    private IEnumerator DisplayScoreCoroutine(GameObject canvas, int myScore, bool destroyParent) {
        TMP_Text text = canvas.GetComponentInChildren<TMP_Text>();
        text.text = myScore.ToString();
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        float val = 0;
        float totalTime = 0.5f;
        float initPosY = canvas.transform.position.y;
        while (val < 1) {
            Mathf.Lerp(1, 0, val);
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1 - val);
            canvas.transform.position = new Vector3(canvas.transform.position.x, initPosY + (0.5f * val), canvas.transform.position.z);
            val += Time.deltaTime / totalTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (destroyParent) {
            Destroy(transform.parent.parent.gameObject);
        } else {
            Destroy(transform.parent.gameObject);
        }
    }
}
