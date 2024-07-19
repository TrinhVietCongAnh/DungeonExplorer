using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// L?p qu?n l� chuy?n c?nh v� hi?u ?ng l�m m?
/// </summary>
public class SceneTransition : MonoBehaviour
{
    // ??i t??ng singleton c?a l?p SceneTransition
    public static SceneTransition instance = null;

    // T?c ?? l�m m?
    const float FadeSpeed = 0.08f;

    bool isFadingIn; // Bi?n ki?m tra xem hi?u ?ng l�m m? ?ang di?n ra hay kh�ng

    // Bi?n l?u tr? h�nh ?nh l�m m? v� m�u c?a n�
    Image _fadeEffectImage;
    Color _fadeEffectColor;

    void Awake()
    {
        // Kh?i t?o singleton
        instance = this;

        // Kh?i t?o
        _fadeEffectImage = GetComponent<Image>();
        _fadeEffectColor = _fadeEffectImage.color;

        // B?t ??u hi?u ?ng l�m m? khi b?t ??u game
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// T?i c?nh m?i v� chuy?n c?nh v?i hi?u ?ng l�m m?
    /// </summary>
    /// <param name="nextScene">T�n c?a c?nh ti?p theo</param>
    public void LoadScene(string nextScene)
    {
        StartCoroutine(FadeEffect(nextScene));
    }

    /// <summary>
    /// Hi?u ?ng l�m m? khi chuy?n c?nh
    /// </summary>
    /// <param name="nextScene">T�n c?a c?nh ti?p theo</param>
    IEnumerator FadeEffect(string nextScene)
    {
        yield return StartCoroutine(FadeOut());
        LoadingSceneManager.LoadScene(nextScene);
    }

    /// <summary>
    /// Hi?u ?ng l�m m? khi v�o c?nh
    /// </summary>
    IEnumerator FadeIn()
    {

        isFadingIn = true; // ?�nh d?u ?ang l�m m?

        float r = _fadeEffectColor.r;
        float g = _fadeEffectColor.g;
        float b = _fadeEffectColor.b;

        float alpha = 1;
        _fadeEffectImage.color = new Color(r, g, b, alpha);

        yield return null;

        // D?n d?n l�m m? ??n khi ?? trong su?t l� 0
        while (alpha > 0f)
        {
            alpha -= FadeSpeed;
            _fadeEffectImage.color = new Color(r, g, b, alpha);
            yield return YieldInstructionCache.WaitForSecondsRealtime(0.01f);
        }

        isFadingIn = false; // K?t th�c l�m m?
    }

    /// <summary>
    /// Hi?u ?ng l�m m? khi ra kh?i c?nh
    /// </summary>
    IEnumerator FadeOut()
    {
        float r = _fadeEffectColor.r;
        float g = _fadeEffectColor.g;
        float b = _fadeEffectColor.b;

        // Ch? ??n khi k?t th�c hi?u ?ng l�m m? tr??c ?�
        while (isFadingIn)
        {
            yield return null;
        }

        float alpha = 0;
        // D?n d?n l�m t?ng ?? trong su?t ??n 1
        while (alpha < 1f)
        {
            alpha += FadeSpeed;
            _fadeEffectImage.color = new Color(r, g, b, alpha);

            yield return YieldInstructionCache.WaitForSecondsRealtime(0.01f);
        }

        // ??i m?t kho?ng th?i gian tr??c khi k?t th�c hi?u ?ng l�m m?
        yield return YieldInstructionCache.WaitForSecondsRealtime(0.07f);
    }
}
