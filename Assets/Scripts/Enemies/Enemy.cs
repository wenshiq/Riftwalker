using UnityEngine;

/// <summary>
/// 敌人：数据驱动，按 EnemyData.behavior 切换行为（近战冲锋 / 远程射击 / 自爆）。
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyData data;
    [SerializeField] private float contactAttackInterval = 0.8f;

    private Rigidbody2D rb;
    private Transform player;
    private int currentHealth;
    private float contactTimer;
    private float shootTimer;
    private float explodeTimer;
    private bool exploding;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // M2 简单做法：靠 Tag 找玩家。后续可换成更规范的单例/事件管理。
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    /// <summary>由刷怪逻辑注入数据，并据此上色（占位美术阶段用颜色区分类型）。</summary>
    public void Init(EnemyData enemyData)
    {
        data = enemyData;
        currentHealth = data.maxHealth;
        GetComponent<SpriteRenderer>().sprite = PlaceholderSprite.Create(data.color, 32);
        if (data.scale != 1f)
            transform.localScale = Vector3.one * data.scale; // Boss 放大（碰撞体随 transform 一起缩放）
    }

    private void Update()
    {
        if (data == null) return;

        contactTimer -= Time.deltaTime;
        shootTimer -= Time.deltaTime;

        if (data.behavior == EnemyBehavior.Shooter) ShooterUpdate();
        else if (data.behavior == EnemyBehavior.Exploder) ExploderUpdate();
    }

    private void FixedUpdate()
    {
        if (data == null || player == null) return;
        if (exploding) return; // 爆炸蓄力时原地不动

        switch (data.behavior)
        {
            case EnemyBehavior.Chaser:
            case EnemyBehavior.Exploder:
                MoveToward((Vector2)player.position);
                break;
            case EnemyBehavior.Shooter:
                KeepRange();
                break;
        }
    }

    private void MoveToward(Vector2 target)
    {
        Vector2 dir = (target - rb.position).normalized;
        rb.MovePosition(rb.position + dir * data.moveSpeed * Time.fixedDeltaTime);
    }

    // 远程敌人保持射程：太远靠近，太近后退
    private void KeepRange()
    {
        Vector2 self = rb.position;
        Vector2 pp = (Vector2)player.position;
        float dist = Vector2.Distance(self, pp);
        Vector2 dir = (pp - self).normalized;

        if (dist > data.shootRange) MoveToward(pp);
        else if (dist < data.shootRange * 0.6f) rb.MovePosition(self - dir * data.moveSpeed * Time.fixedDeltaTime);
    }

    private void ShooterUpdate()
    {
        if (player == null) return;
        Vector2 self = (Vector2)transform.position;
        Vector2 pp = (Vector2)player.position;
        float dist = Vector2.Distance(self, pp);

        if (dist <= data.shootRange && shootTimer <= 0f)
        {
            shootTimer = 1f / data.fireRate;
            Vector2 dir = (pp - self).normalized;
            Bullet.Spawn(self, dir, data.bulletSpeed, data.bulletDamage, new Color(1f, 0.6f, 0.2f), true);
        }
    }

    private void ExploderUpdate()
    {
        if (player == null) return;
        Vector2 self = (Vector2)transform.position;
        Vector2 pp = (Vector2)player.position;

        if (!exploding)
        {
            if (Vector2.Distance(self, pp) <= data.explodeRange)
            {
                exploding = true;
                explodeTimer = data.explodeDelay;
            }
            return;
        }

        explodeTimer -= Time.deltaTime;
        if (explodeTimer <= 0f) Explode();
    }

    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)transform.position, data.explodeRange);
        foreach (Collider2D c in hits)
        {
            if (c.TryGetComponent(out PlayerHealth ph)) ph.TakeDamage(data.explodeDamage);
        }
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 只有近战敌人靠碰撞打人
        if (data == null || data.behavior != EnemyBehavior.Chaser) return;
        if (contactTimer > 0f) return;

        if (collision.collider.TryGetComponent(out PlayerHealth health))
        {
            health.TakeDamage(data.contactDamage);
            contactTimer = contactAttackInterval;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            RoomManager.Instance?.NotifyEnemyDied(this);
            Destroy(gameObject);
        }
    }
}
