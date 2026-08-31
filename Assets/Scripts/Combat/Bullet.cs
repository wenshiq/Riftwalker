using UnityEngine;

/// <summary>
/// 子弹：朝指定方向直线飞行，命中目标造成伤害。
/// hurtsPlayer 区分敌我：玩家子弹打敌人（false），敌人子弹打玩家（true）。
/// M2 仍用运行时工厂 Spawn；M3 换对象池 + prefab。
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2f;

    private Rigidbody2D rb;
    private Vector2 velocity;
    private int damage;
    private bool hurtsPlayer;

    public static Bullet Spawn(Vector2 position, Vector2 direction, float speed, int damage, Color color, bool hurtsPlayer)
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
        bullet.Init(direction, speed, damage, hurtsPlayer);
        return bullet;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 direction, float speed, int damage, bool hurtsPlayer)
    {
        this.damage = damage;
        this.hurtsPlayer = hurtsPlayer;
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
            bool isPlayer = target is PlayerHealth;
            if (isPlayer == hurtsPlayer)
            {
                target.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
