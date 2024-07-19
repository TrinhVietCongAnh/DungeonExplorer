/// <summary>
/// L?p t?nh ch?a d? li?u t�y ch?n
/// </summary>
public static class OptionsData
{
    public static OptionsSaveData optionsSaveData;  // D? li?u t�y ch?n ???c l?u tr? v� qu?n l� b?i l?p n�y
    static bool hasInit;    // Bi?n ki?m tra ?� kh?i t?o hay ch?a

    public static void Init()
    {
        // N?u ch?a kh?i t?o
        if (!hasInit)
        {
            hasInit = true;

            // N?u kh�ng c� d? li?u t�y ch?n, t?o m?i
            optionsSaveData = SaveSystem.OptionLoad();
            if (optionsSaveData == null)
            {
                optionsSaveData = new OptionsSaveData();
            }

            // Kh?i t?o c�c c�i ??t
            VideoSettingsManager.VideoOptionsInit();
            GameInputManager.Init();
            SoundSettingsManager.Init();
            AccessibilitySettingsManager.Init();
            LanguageManager.Init();
        }
    }

    /// <summary>
    /// L?u d? li?u t�y ch?n
    /// </summary>
    public static void OptionsSave()
    {
        SaveSystem.OptionSave(optionsSaveData);
    }
}
