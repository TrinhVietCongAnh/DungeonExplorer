using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// _bulletSpeed: T?c ?? c?a vi�n ??n.
// _hitRange: Ph?m vi ki?m tra va ch?m.
// _offset: ?? l?ch t? v? tr� vi�n ??n ?? ki?m tra va ch?m.
// _damage: L??ng s�t th??ng g�y ra.
// _knockBackForce: L?c ??y l�i khi va ch?m.
// _duration: Th?i gian t?n t?i c?a vi�n ??n.
// _currentDuration: Th?i gian hi?n t?i c?a vi�n ??n.
// _playerLayer, _groundLayer: L?p ?? ki?m tra va ch?m.
// _knockBack: ??i t??ng ?? qu?n l� l?c ??y l�i.
// _anim: Th�nh ph?n Animator c?a vi�n ??n.
// _playerDamage: Th�nh ph?n PlayerDamage c?a ng??i ch?i.
// isHit: Bi?n tr?ng th�i ?? x�c ??nh vi�n ??n c� va ch?m kh�ng.
// angle: G�c c?a vi�n ??n.
// bulletDirection: H??ng di chuy?n c?a vi�n ??n.
// _bulletTransform: Transform c?a vi�n ??n.

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] float _bulletSpeed = 6f;
    [SerializeField] float _hitRange = 1.0f;
    [SerializeField] Vector2 _offset = Vector2.zero;
    [SerializeField] int _damage = 1;
    [SerializeField] float _knockBackForce = 8.0f;
    [SerializeField] float _duration = 3.0f;

    float _currentDuration;

    LayerMask _playerLayer, _groundLayer;
    KnockBack _knockBack;

    Animator _anim;
    PlayerDamage _playerDamage;

    protected bool isHit;
    protected float angle;

    protected Vector3 bulletDirection;
    protected Transform _bulletTransform;

    protected virtual void Awake()
    {
        _anim = GetComponent<Animator>();
        _bulletTransform = GetComponent<Transform>();
        _playerDamage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDamage>();

        _playerLayer = LayerMask.GetMask("Player");
        _groundLayer = LayerMask.GetMask("Ground");

        _knockBack.force = _knockBackForce;
    }

    protected virtual void OnEnable()
    {
        var rotation = _bulletTransform.rotation;
        bulletDirection = rotation * Vector3.left;
        angle = rotation.eulerAngles.z;
        _bulletTransform.rotation = Quaternion.identity;
    }

    protected virtual void Update()
    {
        if (!isHit)
        {
            _bulletTransform.Translate(bulletDirection * _bulletSpeed * Time.deltaTime);

            var center = (Vector2)_bulletTransform.position + _offset;
            var hit = Physics2D.OverlapCircle(center, _hitRange, _playerLayer + _groundLayer);
            if (hit)
            {
                if (hit.CompareTag("Player"))
                {
                    if (!_playerDamage.IsDodged)
                    {
                        _knockBack.direction = angle > 90 && angle < 270 ? 1 : -1;
                        _playerDamage.TakeDamage(_damage, _knockBack);
                    }
                }
                isHit = true;
                _anim.SetTrigger("Hit");
            }

            _currentDuration += Time.deltaTime;
            if (_currentDuration >= _duration)
            {
                ObjectPoolManager.instance.ReturnPoolObject(gameObject);
            }
        }
    }

    void OnDisable()
    {
        isHit = false;
        _currentDuration = 0;
    }

    void OnDrawGizmos()
    {
        var center = (Vector2)transform.position + _offset;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, _hitRange);
    }
}
