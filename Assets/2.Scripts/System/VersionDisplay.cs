using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// L?p n�y hi?n th? phi�n b?n c?a ?ng d?ng l�n m�n h�nh.
/// </summary>
public class VersionDisplay : MonoBehaviour
{
    TextMeshProUGUI _versionText;

    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = "v" + Application.version;
    }
}
