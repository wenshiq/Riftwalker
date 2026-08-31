using UnityEngine;

/// <summary>
/// 玩家射击：鼠标瞄准 + 按住左键连射。
/// 射击方向 = 自身指向鼠标世界坐标的方向。
/// </summary>
public class PlayerShoot : MonoBehaviour
{
    [Header("射击参数")]
    [SerializeField] private float fireRate = 4f;   // 每秒发数
    [SerializeField] private float bulletSpeed = 12f;
    [SerializeField] private int bulletDamage = 10;

    private Camera mainCamera;
    private float fireCooldown;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        fireCooldown -= Time.deltaTime;
        if (Input.GetMouseButton(0) && fireCooldown <= 0f)
            Shoot();
    }

    private void Shoot()
    {
        fireCooldown = 1f / fireRate;
        Vector2 dir = AimDirection();
        Bullet.Spawn(transform.position, dir, bulletSpeed, bulletDamage, Color.white);
    }

    /// <summary>把鼠标屏幕坐标转成世界坐标，得到指向鼠标的方向。</summary>
    private Vector2 AimDirection()
    {
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        return ((Vector2)(mouseWorld - transform.position)).normalized;
    }
}
