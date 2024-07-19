using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Xử lý va chạm và di chuyển tọa độ của các nhân vật trong game.
/// </summary>
public class ActorController : MonoBehaviour
{
    // Liên quan đến va chạm
    [Header("Va chạm")]

    // Cài đặt sử dụng collider
    [SerializeField] bool _useCollider = true;
    [SerializeField] float _skinWidth = 0.001f;
    // Số lượng tia ray
    [SerializeField] int _rayCountX = 3;
    [SerializeField] int _rayCountY = 2;
    // Tọa độ gốc và kích thước của collider
    [SerializeField] Vector2 _colliderCenter = Vector2.zero;
    [SerializeField] Vector2 _colliderSize = Vector2.one;

    [Header("Trọng lực")]
    [SerializeField] bool _useGravity = true;    // Sử dụng trọng lực
    [SerializeField] float _gravityScale = 140f; // Cường độ trọng lực
    [SerializeField] float _maxFallSpeed = 40f;  // Tốc độ rơi tối đa

    float _raySpacingX, _raySpacingY;  // Khoảng cách giữa các tia ray   

    float _velocityX, _velocityY;      // Biến tốc độ
    float _movePosX, _movePosY;        // Biến để xử lý di chuyển thực tế
    float _slideVelocity, _slideDirection, _slideDeceleration;  // Biến để xử lý chuyển động trượt

    float _deltaTime;                  // Biến để cache Time.deltaTime 

    bool _isRoofed, _isGrounded, _isLeftWalled, _isRightWalled; // Kiểm tra trạng thái va chạm

    Vector2 _leftRayOrigin, _rightRayOrigin, _topRayOrigin, _bottomRayOrigin;    // Tọa độ gốc của collider
    Vector2 _colliderSizeStore; // Lưu kích thước của collider

    LayerMask _groundLayer;  // Lớp nền

    Transform _actorTransform;  // Transform của nhân vật

    // Lấy trạng thái va chạm
    public bool IsRoofed => _isRoofed;
    public bool IsGrounded => _isGrounded;
    public bool IsLeftWalled => _isLeftWalled;
    public bool IsRightWalled => _isRightWalled;
    public bool IsWalled => _isLeftWalled || _isRightWalled;

    // Thuộc tính sử dụng trọng lực
    public bool UseGravity
    {
        get
        {
            return _useGravity;
        }
        set
        {
            _useGravity = value;

            if (!value)
            {
                _velocityY = 0;
            }
        }
    }

    // Các thuộc tính liên quan đến trọng lực
    public float GravityScale { get => _gravityScale; set => _gravityScale = value; }
    public float MaxFallSpeed { get => _maxFallSpeed; set => _maxFallSpeed = value; }

    // Các thuộc tính liên quan đến tốc độ
    public float VelocityX { get => _velocityX; set => _velocityX = value; }
    public float VelocityY { get => _velocityY; set => _velocityY = value; }
    public bool IsSliding => _slideVelocity > 0;

    void Start()
    {
        _groundLayer = LayerMask.GetMask("Ground");
        _actorTransform = GetComponent<Transform>();

        // Nếu kích thước của collider bằng 0 trên trục x hoặc y, đặt collider và trọng lực không sử dụng
        if (_colliderSize.x <= 0 || _colliderSize.y <= 0)
        {
            _useCollider = _useGravity = false;
        }
    }

    void Update()
    {
        // Cache Time.deltaTime
        _deltaTime = Time.deltaTime;

        // Áp dụng trọng lực nếu đang sử dụng trọng lực
        if (_useGravity)
        {
            HandleGravity();
        }

        // Chuyển Velocity thành movePos
        _movePosX = (_velocityX + (_slideVelocity * _slideDirection)) * _deltaTime;
        _movePosY = _velocityY * _deltaTime;

        // Cập nhật collider và xử lý va chạm nếu đang sử dụng collider
        if (_useCollider)
        {
            ColliderUpdate();
            HandleCollision();
        }
        // Di chuyển nhân vật bằng movePos
        _actorTransform.Translate(new Vector2(_movePosX, _movePosY));

        // Giảm tốc độ trượt nếu nó lớn hơn một ngưỡng nhất định
        if (_slideVelocity > 3.0f)
        {
            _slideVelocity -= _deltaTime * _slideDeceleration;
            _slideVelocity = Mathf.Clamp(_slideVelocity, 0f, float.MaxValue);
        }
        else
        {
            _slideVelocity = 0;
        }

        // Đặt giá trị di chuyển về 0
        _velocityX = 0;
        _movePosX = _movePosY = 0;
    }

    /// <summary>
    /// Phương thức cập nhật collider theo thời gian thực.
    /// </summary>
    void ColliderUpdate()
    {
        // Tính điểm trung tâm
        float centerX = _actorTransform.position.x + _colliderCenter.x;
        float centerY = _actorTransform.position.y + _colliderCenter.y;

        // Tính nửa kích thước của collider
        var colliderHalfSize = _colliderSize * 0.5f;

        // Lấy tọa độ tối thiểu và tối đa của collider
        var min = new Vector2(centerX - colliderHalfSize.x, centerY - colliderHalfSize.y);
        var max = new Vector2(centerX + colliderHalfSize.x, centerY + colliderHalfSize.y);

        // Cập nhật điểm gốc của các tia ray 
        _leftRayOrigin = new Vector2(min.x, min.y);
        _rightRayOrigin = new Vector2(max.x, min.y);
        _topRayOrigin = new Vector2(min.x, max.y);
        _bottomRayOrigin = new Vector2(min.x, min.y);

        // Cập nhật khoảng cách giữa các tia ray khi kích thước collider thay đổi
        if (_colliderSizeStore != _colliderSize)
        {
            _raySpacingX = _colliderSize.y / (_rayCountX - 1);
            _raySpacingY = _colliderSize.x / (_rayCountY - 1);
            _colliderSizeStore = _colliderSize;
        }
    }

    /// <summary>
    /// Phương thức xử lý va chạm.
    /// </summary>
    void HandleCollision()
    {
        // Va chạm ngang
        if (_rayCountX > 0)
        {
            // Kiểm tra va chạm bên trái và bên phải, sau đó lấy trạng thái va chạm
            _isLeftWalled = _movePosX <= 0 && RayCollision(_rayCountX, -_movePosX, _raySpacingX, _leftRayOrigin, Vector2.left);
            _isRightWalled = _movePosX >= 0 && RayCollision(_rayCountX, _movePosX, _raySpacingX, _rightRayOrigin, Vector2.right);

            // Nếu va chạm, đặt tốc độ X thành 0
            if ((_movePosX < 0 && _isLeftWalled) || (_movePosX > 0 && _isRightWalled))
            {
                _velocityX = 0;
            }
        }
        // Va chạm dọc
        if (_rayCountY > 0)
        {
            // Kiểm tra va chạm trên và dưới, sau đó lấy trạng thái va chạm
            _isRoofed = _movePosY >= 0 && RayCollision(_rayCountY, _movePosY, _raySpacingY, _topRayOrigin, Vector2.up);
            _isGrounded = _movePosY <= 0 && RayCollision(_rayCountY, -_movePosY, _raySpacingY, _bottomRayOrigin, Vector2.down);

            // Nếu va chạm, đặt tốc độ Y thành 0
            if ((_movePosY > 0 && _isRoofed) || (_movePosY < 0 && _isGrounded))
            {
                _velocityY = 0;
            }
        }
    }

    /// <summary>
    /// Phương thức xử lý va chạm với các đối tượng sử dụng tia ray.
    /// </summary>
    /// <param name="rayCount">Số lượng tia ray sử dụng để phát hiện va chạm</param>
    /// <param name="rayLength">Chiều dài của tia ray</param>
    /// <param name="raySpacing">Khoảng cách giữa các tia ray</param>
    /// <param name="rayOrigin">Điểm gốc của tia ray</param>
    /// <param name="rayDirection">Hướng của tia ray</param>
    /// <returns>Trạng thái va chạm</returns>
    bool RayCollision(int rayCount, float rayLength, float raySpacing, Vector2 rayOrigin, Vector2 rayDirection)
    {
        rayLength += _skinWidth + 0.01f;
        bool isHorizontalCollisionCheck = rayDirection.x != 0;

        // Sử dụng nhiều tia ray để xử lý va chạm
        for (int i = 0; i < rayCount; i++)
        {
            Vector2 currentRayOrigin = rayOrigin;
            Vector2 rayOriginDir = isHorizontalCollisionCheck ? Vector2.up : Vector2.right;
            currentRayOrigin += rayOriginDir * (raySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(currentRayOrigin, rayDirection, rayLength, _groundLayer);
#if UNITY_EDITOR
            Debug.DrawRay(currentRayOrigin, rayDirection * rayLength * 3, Color.red);
#endif
            // Nếu phát hiện va chạm, thay đổi giá trị di chuyển và trả về true
            if (hit)
            {
                if (isHorizontalCollisionCheck)
                {
                    _movePosX = (hit.distance - _skinWidth) * rayDirection.x;
                }
                else
                {
                    _movePosY = (hit.distance - _skinWidth) * rayDirection.y;
                }
                return true;
            }
        }

        // Nếu không có va chạm, trả về false
        return false;
    }

    /// <summary>
    /// Phương thức xử lý trọng lực.
    /// </summary>
    void HandleGravity()
    {
        if (_isGrounded) return;

        _velocityY -= _gravityScale * _deltaTime;
        _velocityY = Mathf.Clamp(_velocityY, -_maxFallSpeed, float.MaxValue);
    }

    /// <summary>
    /// Phương thức thực hiện chuyển động trượt.
    /// </summary>
    /// <param name="velocity">Tốc độ trượt</param>
    /// <param name="slideDirection">Hướng trượt</param>
    /// <param name="slideDeceleration">Giảm tốc (giá trị mặc định = 50)</param>
    public void SlideMove(float velocity, float slideDirection, float slideDeceleration = 50f)
    {
        _slideVelocity = velocity;
        _slideDirection = slideDirection;
        _slideDeceleration = slideDeceleration;
    }

    /// <summary>
    /// Phương thức hủy chuyển động trượt.
    /// </summary>
    public void SlideCancle()
    {
        _slideVelocity = 0;
    }


    void OnDrawGizmosSelected()
    {
        float offsetX = transform.position.x + _colliderCenter.x;
        float offsetY = transform.position.y + _colliderCenter.y;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector2(offsetX, offsetY), _colliderSize);
    }
}
