using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Là lớp cơ bản của các nhân vật trong trò chơi, cung cấp các chức năng chung như pooling object, animation, flip trái phải, vv.
/// </summary>
public abstract class Actor : MonoBehaviour
{
    // Danh sách các đối tượng trong pooling
    [SerializeField] private List<PoolObjectData> _poolObjectDataList;

    protected float deltaTime;      // Biến float để lưu trữ Time.deltaTime
    protected bool isDead;          // Kiểm tra xem nhân vật đã chết chưa

    protected Transform actorTransform;
    protected ActorController controller = null;

    protected Animator animator;
    protected Dictionary<string, int> animationHash = new Dictionary<string, int>();

    protected bool FacingRight { get; private set; }

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        actorTransform = GetComponent<Transform>();

        if (TryGetComponent(out ActorController controller))
        {
            this.controller = controller;
        }

        // Khởi tạo object pooling
        foreach (var poolObject in _poolObjectDataList)
        {
            ObjectPoolManager.instance.CreatePool(poolObject);
        }

        // Đặt hướng ban đầu
        FacingRight = actorTransform.localScale.x > 0;
    }

    #region Animator

    /// <summary>
    /// Phương thức này lấy và cache hash code của tên animation đã cho.
    /// </summary>
    /// <param name="name">Tên animation</param>
    /// <returns>Hash code của animation</returns>
    protected int GetAnimationHash(string name)
    {
        // Nếu hash của animation chưa tồn tại, tạo mới và trả về
        if (!animationHash.TryGetValue(name, out int hash))
        {
            animationHash.Add(name, Animator.StringToHash(name));
            hash = Animator.StringToHash(name);
        }

        return hash;
    }

    /// <summary>
    /// Phương thức này lấy thời gian chuẩn hóa của animation hiện tại.
    /// </summary>
    /// <returns>Thời gian chuẩn hóa hiện tại của animation</returns>
    protected float GetAnimatorNormalizedTime() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

    /// <summary>
    /// Phương thức này kiểm tra xem animation hiện tại đã kết thúc chưa.
    /// </summary>
    /// <returns>Trạng thái kết thúc của animation</returns>
    protected bool IsAnimationEnded() => GetAnimatorNormalizedTime() >= 0.99f;

    /// <summary>
    /// Phương thức này kiểm tra xem thời gian chuẩn hóa của animation hiện tại có nằm trong khoảng đã cho không.
    /// </summary>
    /// <param name="minTime">Thời gian tối thiểu (theo chuẩn hóa)</param>
    /// <param name="maxTime">Thời gian tối đa (theo chuẩn hóa)</param>
    /// <returns>True nếu thời gian chuẩn hóa của animation hiện tại nằm trong khoảng đã cho, ngược lại là False</returns>
    protected bool IsAnimatorNormalizedTimeInBetween(float minTime, float maxTime)
    {
        float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        return normalizedTime >= minTime && normalizedTime <= maxTime;
    }

    #endregion

    #region Actor Flip

    /// <summary>
    /// Phương thức này đảo ngược hướng của nhân vật theo trục trái phải.
    /// </summary>
    protected void Flip()
    {
        FacingRight = !FacingRight;

        int newScaleX = FacingRight ? 1 : -1;
        actorTransform.localScale = new Vector2(newScaleX, 1);
    }

    /// <summary>
    /// Phương thức này đặt hướng của nhân vật dựa trên giá trị đầu vào. Nếu giá trị là 0, hướng sẽ không thay đổi.
    /// </summary>
    /// <param name="direction">Hướng muốn đặt (giá trị âm: trái, giá trị dương: phải)</param>
    protected void SetFacingDirection(float direction)
    {
        // Nếu giá trị không phải là 0, đặt hướng mới và cập nhật FacingRight
        if (direction != 0)
        {
            actorTransform.localScale = new Vector2(Mathf.Sign(direction), 1);
            FacingRight = actorTransform.localScale.x > 0;
        }
    }

    #endregion
}
