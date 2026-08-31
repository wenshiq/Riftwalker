using UnityEngine;

/// <summary>
/// 玩家射击：鼠标瞄准 + 按住左键连射，支持弹幕分裂（bulletCount）。
/// 射速/伤害/弹数从 PlayerStats 读取，可被强化修改。
/// </summary>
public class PlayerShoot : MonoBehaviour
{
    [Header("射击参数")]
    [SerializeField] private float bulletSpeed = 12f;

    private Camera mainCamera;
    private PlayerStats stats;
    private float fireCooldown;

    private void Awake()
    {
        mainCamera = Camera.main;
        stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        fireCooldown -= Time.deltaTime;
        if (Input.GetMouseButton(0) && fireCooldown <= 0f)
            Shoot();
    }

    private void Shoot()
    {
        float rate = stats != null ? stats.fireRate : 4f;
        fireCooldown = 1f / rate;

        int damage = stats != null ? stats.bulletDamage : 10;
        int count = Mathf.Max(1, stats != null ? stats.bulletCount : 1);

        Vector2 baseDir = AimDirection();
        for (int i = 0; i < count; i++)
        {
            Vector2 dir = baseDir;
            if (count > 1)
            {
                float spread = 15f; // 相邻子弹夹角（度）
                float angle = (i - (count - 1) / 2f) * spread;
                dir = (Vector2)(Quaternion.Euler(0, 0, angle) * (Vector3)baseDir);
            }
            Bullet.Spawn(transform.position, dir, bulletSpeed, damage, Color.white, false);
        }
    }

    /// <summary>把鼠标屏幕坐标转成世界坐标，得到指向鼠标的方向。</summary>
    private Vector2 AimDirection()
    {
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        return ((Vector2)(mouseWorld - transform.position)).normalized;
    }
}
