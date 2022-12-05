using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectorDisplay : MonoBehaviour
{
    public ScriptableObjectListSO availableTablewares;
    public Image before;
    public Image current;
    public Image after;

    void Start() {

    }

    void Update() {
        int beforeCursor = (availableTablewares.cursor - 1) < 0 ? availableTablewares.list.Count - 1 : (availableTablewares.cursor - 1);
        int afterCursor = (availableTablewares.cursor + 1) % availableTablewares.list.Count;
        before.sprite = ((TablewareSO)availableTablewares.list[beforeCursor]).selectorIcon;
        after.sprite = ((TablewareSO)availableTablewares.list[afterCursor]).selectorIcon;
        current.sprite = ((TablewareSO)availableTablewares.list[availableTablewares.cursor]).selectorIcon;
    }
}
