using UnityEngine;

/// <summary>
/// 集中管理玩家可被强化修改的属性：强化系统改这里，控制器/射击从这里读。
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("基础属性")]
    public float moveSpeed = 5f;
    public float fireRate = 4f;
    public int bulletDamage = 10;
    public int bulletCount = 1;

    private PlayerHealth health;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
    }

    /// <summary>应用一个强化项。</summary>
    public void ApplyUpgrade(UpgradeData up)
    {
        if (up == null) return;

        switch (up.type)
        {
            case UpgradeType.MoveSpeed: moveSpeed += up.value; break;
            case UpgradeType.FireRate: fireRate += up.value; break;
            case UpgradeType.Damage: bulletDamage += (int)up.value; break;
            case UpgradeType.MultiShot: bulletCount += (int)up.value; break;
            case UpgradeType.MaxHealth:
                if (health != null) health.IncreaseMaxHealth((int)up.value);
                break;
        }
    }
}
