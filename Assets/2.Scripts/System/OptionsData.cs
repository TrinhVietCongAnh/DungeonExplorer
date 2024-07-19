/// <summary>
/// L?p t?nh ch?a d? li?u tùy ch?n
/// </summary>
public static class OptionsData
{
    public static OptionsSaveData optionsSaveData;  // D? li?u tùy ch?n ???c l?u tr? và qu?n lý b?i l?p này
    static bool hasInit;    // Bi?n ki?m tra ?ã kh?i t?o hay ch?a

    public static void Init()
    {
        // N?u ch?a kh?i t?o
        if (!hasInit)
        {
            hasInit = true;

            // N?u không có d? li?u tùy ch?n, t?o m?i
            optionsSaveData = SaveSystem.OptionLoad();
            if (optionsSaveData == null)
            {
                optionsSaveData = new OptionsSaveData();
            }

            // Kh?i t?o các cài ??t
            VideoSettingsManager.VideoOptionsInit();
            GameInputManager.Init();
            SoundSettingsManager.Init();
            AccessibilitySettingsManager.Init();
            LanguageManager.Init();
        }
    }

    /// <summary>
    /// L?u d? li?u tùy ch?n
    /// </summary>
    public static void OptionsSave()
    {
        SaveSystem.OptionSave(optionsSaveData);
    }
}
