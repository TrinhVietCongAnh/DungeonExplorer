using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// L?p qu?n lý chuy?n c?nh và hi?u ?ng làm m?
/// </summary>
public class SceneTransition : MonoBehaviour
{
    // ??i t??ng singleton c?a l?p SceneTransition
    public static SceneTransition instance = null;

    // T?c ?? làm m?
    const float FadeSpeed = 0.08f;

    bool isFadingIn; // Bi?n ki?m tra xem hi?u ?ng làm m? ?ang di?n ra hay không

    // Bi?n l?u tr? hình ?nh làm m? và màu c?a nó
    Image _fadeEffectImage;
    Color _fadeEffectColor;

    void Awake()
    {
        // Kh?i t?o singleton
        instance = this;

        // Kh?i t?o
        _fadeEffectImage = GetComponent<Image>();
        _fadeEffectColor = _fadeEffectImage.color;

        // B?t ??u hi?u ?ng làm m? khi b?t ??u game
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// T?i c?nh m?i và chuy?n c?nh v?i hi?u ?ng làm m?
    /// </summary>
    /// <param name="nextScene">Tên c?a c?nh ti?p theo</param>
    public void LoadScene(string nextScene)
    {
        StartCoroutine(FadeEffect(nextScene));
    }

    /// <summary>
    /// Hi?u ?ng làm m? khi chuy?n c?nh
    /// </summary>
    /// <param name="nextScene">Tên c?a c?nh ti?p theo</param>
    IEnumerator FadeEffect(string nextScene)
    {
        yield return StartCoroutine(FadeOut());
        LoadingSceneManager.LoadScene(nextScene);
    }

    /// <summary>
    /// Hi?u ?ng làm m? khi vào c?nh
    /// </summary>
    IEnumerator FadeIn()
    {

        isFadingIn = true; // ?ánh d?u ?ang làm m?

        float r = _fadeEffectColor.r;
        float g = _fadeEffectColor.g;
        float b = _fadeEffectColor.b;

        float alpha = 1;
        _fadeEffectImage.color = new Color(r, g, b, alpha);

        yield return null;

        // D?n d?n làm m? ??n khi ?? trong su?t là 0
        while (alpha > 0f)
        {
            alpha -= FadeSpeed;
            _fadeEffectImage.color = new Color(r, g, b, alpha);
            yield return YieldInstructionCache.WaitForSecondsRealtime(0.01f);
        }

        isFadingIn = false; // K?t thúc làm m?
    }

    /// <summary>
    /// Hi?u ?ng làm m? khi ra kh?i c?nh
    /// </summary>
    IEnumerator FadeOut()
    {
        float r = _fadeEffectColor.r;
        float g = _fadeEffectColor.g;
        float b = _fadeEffectColor.b;

        // Ch? ??n khi k?t thúc hi?u ?ng làm m? tr??c ?ó
        while (isFadingIn)
        {
            yield return null;
        }

        float alpha = 0;
        // D?n d?n làm t?ng ?? trong su?t ??n 1
        while (alpha < 1f)
        {
            alpha += FadeSpeed;
            _fadeEffectImage.color = new Color(r, g, b, alpha);

            yield return YieldInstructionCache.WaitForSecondsRealtime(0.01f);
        }

        // ??i m?t kho?ng th?i gian tr??c khi k?t thúc hi?u ?ng làm m?
        yield return YieldInstructionCache.WaitForSecondsRealtime(0.07f);
    }
}
