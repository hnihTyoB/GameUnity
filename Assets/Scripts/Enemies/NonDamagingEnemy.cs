using UnityEngine;

/// <summary>
/// Component để đánh dấu enemy không gây damage/knockback
/// Dùng cho game phòng chống bạo lực - Shadow Ghost chỉ làm chậm, không gây hại trực tiếp
/// </summary>
public class NonDamagingEnemy : MonoBehaviour
{
    // Component này chỉ cần tồn tại trên GameObject
    // PlayerHealth sẽ check component này để quyết định có apply damage/knockback hay không
}

