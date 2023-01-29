using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateAmmoUI : MonoBehaviour {

    public ScriptableObjectListSO availableTablewares;
    public TMP_Text text;
    public Image icon;

    void Update() {
        text.text = ((TablewareSO)availableTablewares.list[availableTablewares.cursor]).ammo.ToString();
        icon.sprite = ((TablewareSO)availableTablewares.list[availableTablewares.cursor]).selectorIcon;
    }
}
