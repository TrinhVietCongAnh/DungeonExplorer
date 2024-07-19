using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// L?p qu?n lý quá trình t?i trang
/// </summary>
public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene; // Trang k? ti?p c?n t?i

    void Start()
    {
        // Kh?i ??ng quá trình
        StartCoroutine(LoadSceneCoroutine());
    }

    /// <summary>
    /// T?i trang ???c ch? ??nh và gi? trang hi?n t?i ?? gi?m thi?u hi?u ?ng nh?p nháy, gi?m t?i GC.
    /// </summary>
    /// <param name="sceneName">Tên c?a trang c?n t?i</param>
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

        // B?t ??u t?i trang k? ti?p nh?ng không chuy?n ??n ngay
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextScene);
        asyncOperation.allowSceneActivation = false;

        // ??i cho ??n khi quá trình t?i hoàn t?t
        while (!asyncOperation.isDone)
        {
            yield return null;

            // Khi ti?n trình t?i g?n hoàn t?t (0.9), cho phép chuy?n trang và thi?t l?p l?i Time.timeScale
            if (asyncOperation.progress >= 0.9f)
            {
                Time.timeScale = 1f;
                asyncOperation.allowSceneActivation = true;
            }
        }
    }
}
