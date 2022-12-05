using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayDate : MonoBehaviour
{
    private string[] months = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
    public TMP_Text text;
    void Start() {
        DateTime now = DateTime.Now;
        text.text = months[now.Month - 1] + "" + now.Day + "'" + now.Year;
    }
}
