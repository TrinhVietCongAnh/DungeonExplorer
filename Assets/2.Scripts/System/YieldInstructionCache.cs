using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// L?p này l?u tr? và cung c?p các ??i t??ng YieldInstruction nh? WaitForSeconds và WaitForSecondsRealtime ?? t?i ?u hóa vi?c s? d?ng chúng trong game.
/// </summary>
public static class YieldInstructionCache
{
    /// <summary>
    /// L?p so sánh IEqualityComparer cho ki?u float.
    /// </summary>
    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            return x == y;
        }
        int IEqualityComparer<float>.GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    // Dictionary l?u tr? các ??i t??ng WaitForSeconds và WaitForSecondsRealtime
    static readonly Dictionary<float, WaitForSeconds> _timeInterval =
                new Dictionary<float, WaitForSeconds>(new FloatComparer());

    static readonly Dictionary<float, WaitForSecondsRealtime> _realTimeInterval =
                new Dictionary<float, WaitForSecondsRealtime>(new FloatComparer());

    /// <summary>
    /// Tr? v? ??i t??ng WaitForSeconds v?i th?i gian nh?t ??nh.
    /// </summary>
    /// <param name="seconds">Th?i gian ch? (giây)</param>
    /// <returns>??i t??ng WaitForSeconds</returns>
    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds waitForSeconds;

        // N?u th?i gian ?ã ???c l?u trong Dictionary, tr? v? ??i t??ng ?ó; n?u không, t?o m?i và thêm vào Dictionary
        if (!_timeInterval.TryGetValue(seconds, out waitForSeconds))
        {
            _timeInterval.Add(seconds, waitForSeconds = new WaitForSeconds(seconds));
        }
        return waitForSeconds;
    }

    /// <summary>
    /// Tr? v? ??i t??ng WaitForSecondsRealtime v?i th?i gian nh?t ??nh.
    /// </summary>
    /// <param name="seconds">Th?i gian ch? (giây)</param>
    /// <returns>??i t??ng WaitForSecondsRealtime</returns>
    public static WaitForSecondsRealtime WaitForSecondsRealtime(float seconds)
    {
        WaitForSecondsRealtime waitForSecondsRealtime;

        // N?u th?i gian ?ã ???c l?u trong Dictionary, tr? v? ??i t??ng ?ó; n?u không, t?o m?i và thêm vào Dictionary
        if (!_realTimeInterval.TryGetValue(seconds, out waitForSecondsRealtime))
        {
            _realTimeInterval.Add(seconds, waitForSecondsRealtime = new WaitForSecondsRealtime(seconds));
        }
        return waitForSecondsRealtime;
    }
}
