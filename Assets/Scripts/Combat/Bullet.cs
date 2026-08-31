using UnityEngine;

/// <summary>
/// 子弹：朝指定方向直线飞行，命中带 IDamageable 的目标造成伤害。
/// M1 用运行时工厂 Spawn 直接生成（不用 prefab）；M2 会改成对象池 + prefab。
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2f;

    private Rigidbody2D rb;
    private Vector2 velocity;
    private int damage;

    /// <summary>运行时创建一个子弹对象（M1 的占位做法，M2 换对象池）。</summary>
    public static Bullet Spawn(Vector2 position, Vector2 direction, float speed, int damage, Color color)
    {
        GameObject go = new GameObject("Bullet");
        go.transform.position = position;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = PlaceholderSprite.Create(color, 16);
        sr.sortingOrder = 10;

        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.15f;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        Bullet bullet = go.AddComponent<Bullet>();
        bullet.Init(direction, speed, damage);
        return bullet;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 direction, float speed, int damage)
    {
        this.damage = damage;
        velocity = direction * speed;
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageable target))
        {
            // 玩家自己的子弹不能打到自己
            if (target is PlayerHealth) return;

            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
