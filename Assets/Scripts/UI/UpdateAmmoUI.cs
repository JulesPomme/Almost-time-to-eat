using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateAmmoUI : MonoBehaviour {

    public ScriptableObjectListSO availableTablewares;
    private TMP_Text text;

    void Start() {
        text = GetComponent<TMP_Text>();
    }

    void Update() {
        text.text = ((TablewareSO)availableTablewares.list[availableTablewares.cursor]).ammo.ToString();
    }
}
