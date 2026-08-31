using UnityEngine;

/// <summary>敌人行为类型。</summary>
public enum EnemyBehavior
{
    Chaser,   // 近战冲锋
    Shooter,  // 远程射击
    Exploder  // 自爆
}

/// <summary>
/// 敌人数据（ScriptableObject）：把敌人属性抽成资产，实现数据驱动。
/// 由 Riftwalker/一键搭建 M2 场景 自动生成到 Assets/Data，
/// 也可手动右键 → Create → Riftwalker → EnemyData 创建。
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "Riftwalker/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string displayName = "敌人";
    public EnemyBehavior behavior = EnemyBehavior.Chaser;
    public int maxHealth = 30;
    public float moveSpeed = 2.5f;
    public int contactDamage = 10;
    public Color color = new Color(1f, 0.35f, 0.35f);
    public bool isBoss = false;  // 标记为 Boss（Boss 房间单独刷一只）
    public float scale = 1f;     // 体型缩放（Boss 放大用）

    [Header("远程（Shooter）")]
    public float shootRange = 8f;
    public float fireRate = 1f;
    public float bulletSpeed = 6f;
    public int bulletDamage = 8;

    [Header("自爆（Exploder）")]
    public float explodeRange = 1.8f;
    public int explodeDamage = 30;
    public float explodeDelay = 0.8f;
}
