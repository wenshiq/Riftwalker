using UnityEngine;

/// <summary>
/// 基础敌人：追向玩家，碰到玩家造成接触伤害（带伤害间隔）。
/// M1 只有这一个敌人类型；M2 会用 ScriptableObject 数据驱动 + 更多行为。
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IDamageable
{
    [Header("属性")]
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private int contactDamage = 10;
    [SerializeField] private float attackInterval = 0.8f; // 两次接触伤害的间隔

    private Rigidbody2D rb;
    private Transform player;
    private int currentHealth;
    private float damageTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        // M1 简单做法：靠 Tag 找玩家。M2 换成更规范的单例 / 事件管理。
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void FixedUpdate()
    {
        if (player == null) return;
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
    }

    private void Update()
    {
        if (damageTimer > 0f) damageTimer -= Time.deltaTime;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (damageTimer > 0f) return;
        if (collision.collider.TryGetComponent(out PlayerHealth health))
        {
            health.TakeDamage(contactDamage);
            damageTimer = attackInterval;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Destroy(gameObject);
    }
}
