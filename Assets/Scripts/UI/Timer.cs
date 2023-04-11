using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour {
    public IntegerSO totalTimeInSeconds;
    public FloatSO currentTime;
    public AudioSource tick;

    private TMP_Text text;
    private bool[] lastCountDownHasTicked;

    void Start() {
        text = GetComponent<TMP_Text>();
        Init();
    }

    private void Init() {
        currentTime.value = totalTimeInSeconds.value;
        lastCountDownHasTicked = new bool[] { false, false, false, false, false, false, false, false, false, false };
    }

    void Update() {
        currentTime.value -= Time.deltaTime;
        currentTime.value = Mathf.Clamp(currentTime.value, 0, totalTimeInSeconds.value);
        int intTime = Mathf.CeilToInt(currentTime.value);
        int minutes = (intTime / 60);
        int seconds = (intTime % 60);
        text.text = minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
        if (intTime < 11 && intTime > 0 && !lastCountDownHasTicked[intTime - 1]) {
            tick.Play();
            lastCountDownHasTicked[intTime - 1] = true;
            int prev = intTime % 10;
            lastCountDownHasTicked[prev] = false;
        }
    }
}
