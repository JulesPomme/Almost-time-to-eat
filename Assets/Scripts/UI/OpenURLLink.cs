using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using TMPro;

public class OpenURLLink : MonoBehaviour
{
    public TMP_Text url;

    public void OpenLinkJSPlugin() {
#if !UNITY_EDITOR
		openWindow(url.text);
#endif
    }

    [DllImport("__Internal")]
    private static extern void openWindow(string url);
}
