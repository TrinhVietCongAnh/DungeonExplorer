using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Là lớp tĩnh quản lý và cài đặt tùy chọn truy cập.
/// </summary>
public static class AccessibilitySettingsManager
{
    public static float gamepadVibration = 1.0f;    // Cường độ rung của tay cầm
    public static bool screenShake = true;          // Có rung màn hình không
    public static bool screenFlashes = true;        // Có chớp màn hình không

    /// <summary>
    /// Phương thức tĩnh để khởi tạo trình quản lý truy cập.
    /// </summary>
    public static void Init()
    {
        gamepadVibration = OptionsData.optionsSaveData.gamepadVibration;
        screenShake = OptionsData.optionsSaveData.screenShake;
        screenFlashes = OptionsData.optionsSaveData.screenFlashes;
    }

    /// <summary>
    /// Phương thức tĩnh để thiết lập cường độ rung của tay cầm.
    /// </summary>
    /// <param name="increase">Có tăng cường độ rung không</param>
    public static void SetGamepadVibration(bool increase)
    {
        // Thiết lập cường độ rung
        gamepadVibration += increase ? 0.2f : -0.2f;
        gamepadVibration = Mathf.Clamp(gamepadVibration, 0f, 1f);

        // Rung tay cầm để người dùng cảm nhận được cường độ rung hiện tại
        GamepadVibrationManager.instance.GamepadRumbleStart(gamepadVibration, 0.05f);

        // Lưu cường độ rung hiện tại vào dữ liệu tùy chọn
        OptionsData.optionsSaveData.gamepadVibration = gamepadVibration;
    }

    /// <summary>
    /// Phương thức tĩnh để lấy chuỗi UI hiển thị cường độ rung của tay cầm.
    /// </summary>
    /// <returns>Chuỗi UI hiển thị cường độ rung của tay cầm</returns>
    public static string GetGamepadVibrationToUI()
    {
        int vibration = Mathf.RoundToInt(gamepadVibration * 5);
        StringBuilder vibrationToText = new StringBuilder();
        for (int i = 0; i < 5; i++)
        {
            if (i < vibration)
            {
                vibrationToText.Append("■");
            }
            else
            {
                vibrationToText.Append("□");
            }
        }
        return vibrationToText.ToString();
    }

    /// <summary>
    /// Phương thức tĩnh để thiết lập việc sử dụng rung màn hình.
    /// </summary>
    public static void SetScreenShake()
    {
        screenShake = !screenShake;
        OptionsData.optionsSaveData.screenShake = screenShake;
    }

    /// <summary>
    /// Phương thức để lấy chuỗi UI cho trạng thái sử dụng rung màn hình.
    /// </summary>
    /// <returns>Chuỗi UI cho trạng thái sử dụng rung màn hình</returns>
    public static string GetScreenShakeToggle()
    {
        return screenShake ? LanguageManager.GetText("Enabled") : LanguageManager.GetText("Disabled");
    }

    /// <summary>
    /// Phương thức tĩnh để thiết lập việc sử dụng chớp màn hình.
    /// </summary>
    public static void SetScreenFlashes()
    {
        screenFlashes = !screenFlashes;
        OptionsData.optionsSaveData.screenFlashes = screenFlashes;
    }

    /// <summary>
    /// Phương thức để lấy chuỗi UI cho trạng thái sử dụng chớp màn hình.
    /// </summary>
    /// <returns>Chuỗi UI cho trạng thái sử dụng chớp màn hình</returns>
    public static string GetScreenFlashesToggle()
    {
        return screenFlashes == true ? LanguageManager.GetText("Enabled") : LanguageManager.GetText("Disabled");
    }
}
