using System.Collections;

/// <summary>
/// Interface ch?a các ph??ng th?c và s? ki?n ?? x? lý thi?t h?i cho nhân v?t trong game.
/// </summary>
public interface IActorDamage
{
    public delegate void KnockbackEventHandler(KnockBack knockBack);    // S? ki?n x? lý khi b? ??y lùi
    public delegate IEnumerator DiedEventHandler();                     // S? ki?n x? lý khi ch?t

    public int CurrentHealth { get; set; }    // Thu?c tính s?c kh?e hi?n t?i
    public float GetHealthPercent();          // Ph??ng th?c tr? v? ph?n tr?m s?c kh?e hi?n t?i

    public void TakeDamage(int damage, KnockBack knockBack);    // Ph??ng th?c x? lý thi?t h?i và ??y lùi
}
