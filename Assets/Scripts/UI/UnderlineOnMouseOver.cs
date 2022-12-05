using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UnderlineOnMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text text;
    public void OnPointerEnter(PointerEventData eventData) {
        text.fontStyle = TMPro.FontStyles.Underline;
    }

    public void OnPointerExit(PointerEventData eventData) {
        text.fontStyle &= ~TMPro.FontStyles.Underline;
    }

}
