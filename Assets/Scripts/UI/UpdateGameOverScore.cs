using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateGameOverScore : MonoBehaviour
{
    public TablewareInstanceListSO instantiatedTableware;
    public ScriptableObjectListSO tablewareWaveListSO;
    public IntegerSO score;
    public FloatSO currentTime;
    public IntegerSO brokenCount;
    public IntegerSO nbCompletedTables;

    public TMP_Text scoreText;
    public TMP_Text nbCompletedTableText;
    public TMP_Text tablewareUsedText;
    public TMP_Text tablewareBrokenText;
    public TMP_Text remainingTimeText;

    private void OnEnable() {

        int perfectNbTableWare = 0;
        foreach (ScriptableObject so in tablewareWaveListSO.list) {
            TablewareZoneSO wave = (TablewareZoneSO)so;
            perfectNbTableWare += wave.zonePrefab.transform.childCount;
        }
        scoreText.text = score.value.ToString();
        nbCompletedTableText.text = nbCompletedTables.value.ToString();
        tablewareUsedText.text = instantiatedTableware.list.Count.ToString();
        tablewareBrokenText.text = brokenCount.value.ToString();
        remainingTimeText.text = Mathf.CeilToInt(currentTime.value).ToString();
    }
}
