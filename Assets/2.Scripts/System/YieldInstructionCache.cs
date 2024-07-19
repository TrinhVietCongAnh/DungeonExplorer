using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// L?p n�y l?u tr? v� cung c?p c�c ??i t??ng YieldInstruction nh? WaitForSeconds v� WaitForSecondsRealtime ?? t?i ?u h�a vi?c s? d?ng ch�ng trong game.
/// </summary>
public static class YieldInstructionCache
{
    /// <summary>
    /// L?p so s�nh IEqualityComparer cho ki?u float.
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

    // Dictionary l?u tr? c�c ??i t??ng WaitForSeconds v� WaitForSecondsRealtime
    static readonly Dictionary<float, WaitForSeconds> _timeInterval =
                new Dictionary<float, WaitForSeconds>(new FloatComparer());

    static readonly Dictionary<float, WaitForSecondsRealtime> _realTimeInterval =
                new Dictionary<float, WaitForSecondsRealtime>(new FloatComparer());

    /// <summary>
    /// Tr? v? ??i t??ng WaitForSeconds v?i th?i gian nh?t ??nh.
    /// </summary>
    /// <param name="seconds">Th?i gian ch? (gi�y)</param>
    /// <returns>??i t??ng WaitForSeconds</returns>
    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds waitForSeconds;

        // N?u th?i gian ?� ???c l?u trong Dictionary, tr? v? ??i t??ng ?�; n?u kh�ng, t?o m?i v� th�m v�o Dictionary
        if (!_timeInterval.TryGetValue(seconds, out waitForSeconds))
        {
            _timeInterval.Add(seconds, waitForSeconds = new WaitForSeconds(seconds));
        }
        return waitForSeconds;
    }

    /// <summary>
    /// Tr? v? ??i t??ng WaitForSecondsRealtime v?i th?i gian nh?t ??nh.
    /// </summary>
    /// <param name="seconds">Th?i gian ch? (gi�y)</param>
    /// <returns>??i t??ng WaitForSecondsRealtime</returns>
    public static WaitForSecondsRealtime WaitForSecondsRealtime(float seconds)
    {
        WaitForSecondsRealtime waitForSecondsRealtime;

        // N?u th?i gian ?� ???c l?u trong Dictionary, tr? v? ??i t??ng ?�; n?u kh�ng, t?o m?i v� th�m v�o Dictionary
        if (!_realTimeInterval.TryGetValue(seconds, out waitForSecondsRealtime))
        {
            _realTimeInterval.Add(seconds, waitForSecondsRealtime = new WaitForSecondsRealtime(seconds));
        }
        return waitForSecondsRealtime;
    }
}
