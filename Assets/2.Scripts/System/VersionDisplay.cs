using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// L?p này hi?n th? phiên b?n c?a ?ng d?ng lên màn hình.
/// </summary>
public class VersionDisplay : MonoBehaviour
{
    TextMeshProUGUI _versionText;

    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = "v" + Application.version;
    }
}
