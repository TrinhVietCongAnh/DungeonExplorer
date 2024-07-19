using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageMusic : MonoBehaviour
{
    public AudioClip stageMusic;     // �m thanh c?a giai ?o?n
    public float volume = 1.0f;      // �m l??ng m?c ??nh

    void Start()
    {
        // N?u kh�ng c� �m thanh giai ?o?n ???c ch? ??nh
        if (stageMusic == null)
        {
            SoundManager.instance.MusicStop(); // D?ng ph�t �m thanh
        }
        // Ph�t �m thanh giai ?o?n v?i �m l??ng v� th?i l??ng ???c ch? ??nh
        SoundManager.instance.MusicPlay(stageMusic, volume);    
    }
}
