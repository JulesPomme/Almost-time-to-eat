using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateScoreUI : MonoBehaviour
{
    public IntegerSO score;
    private TMP_Text text;

    void Start() {
        text = GetComponent<TMP_Text>();
    }

    void Update() {
        text.text = "Score: " + score.value;
    }
}
