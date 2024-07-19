using System.Collections;

/// <summary>
/// Interface ch?a c�c ph??ng th?c v� s? ki?n ?? x? l� thi?t h?i cho nh�n v?t trong game.
/// </summary>
public interface IActorDamage
{
    public delegate void KnockbackEventHandler(KnockBack knockBack);    // S? ki?n x? l� khi b? ??y l�i
    public delegate IEnumerator DiedEventHandler();                     // S? ki?n x? l� khi ch?t

    public int CurrentHealth { get; set; }    // Thu?c t�nh s?c kh?e hi?n t?i
    public float GetHealthPercent();          // Ph??ng th?c tr? v? ph?n tr?m s?c kh?e hi?n t?i

    public void TakeDamage(int damage, KnockBack knockBack);    // Ph??ng th?c x? l� thi?t h?i v� ??y l�i
}
