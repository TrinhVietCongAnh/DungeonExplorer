using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// L?p qu?n l� qu� tr�nh t?i trang
/// </summary>
public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene; // Trang k? ti?p c?n t?i

    void Start()
    {
        // Kh?i ??ng qu� tr�nh
        StartCoroutine(LoadSceneCoroutine());
    }

    /// <summary>
    /// T?i trang ???c ch? ??nh v� gi? trang hi?n t?i ?? gi?m thi?u hi?u ?ng nh?p nh�y, gi?m t?i GC.
    /// </summary>
    /// <param name="sceneName">T�n c?a trang c?n t?i</param>
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
        System.GC.Collect();
    }

    /// <summary>
    /// Coroutine ?? t?i trang
    /// </summary>
    IEnumerator LoadSceneCoroutine()
    {
        // ??i m?t frame
        yield return null;

        // B?t ??u t?i trang k? ti?p nh?ng kh�ng chuy?n ??n ngay
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextScene);
        asyncOperation.allowSceneActivation = false;

        // ??i cho ??n khi qu� tr�nh t?i ho�n t?t
        while (!asyncOperation.isDone)
        {
            yield return null;

            // Khi ti?n tr�nh t?i g?n ho�n t?t (0.9), cho ph�p chuy?n trang v� thi?t l?p l?i Time.timeScale
            if (asyncOperation.progress >= 0.9f)
            {
                Time.timeScale = 1f;
                asyncOperation.allowSceneActivation = true;
            }
        }
    }
}
