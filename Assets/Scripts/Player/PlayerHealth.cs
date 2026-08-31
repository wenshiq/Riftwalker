using System;
using UnityEngine;

/// <summary>
/// 玩家生命：负责血量、无敌帧（受击 / 闪避）、受伤闪烁、死亡。
/// </summary>
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("生命")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float hitInvincibleTime = 0.5f;   // 受击后的短暂无敌，避免被群殴瞬秒
    [SerializeField] private float flashInterval = 0.08f;      // 无敌闪烁频率

    public int CurrentHealth { get; private set; }
    public event Action OnDied;

    private float invincibleTimer;
    private SpriteRenderer sprite;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (invincibleTimer > 0f)
        {
            invincibleTimer -= Time.deltaTime;
            if (sprite != null)
                sprite.enabled = Mathf.FloorToInt(Time.time / flashInterval) % 2 == 0;
        }
        else if (sprite != null && !sprite.enabled)
        {
            sprite.enabled = true;
        }
    }

    public void TakeDamage(int amount)
    {
        if (invincibleTimer > 0f) return; // 无敌期间不掉血

        CurrentHealth -= amount;
        SetInvincible(hitInvincibleTime);

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            OnDied?.Invoke();
            gameObject.SetActive(false);
        }
    }

    /// <summary>设置无敌时间（取较大值），用于受击保护或闪避无敌帧。</summary>
    public void SetInvincible(float duration)
    {
        invincibleTimer = Mathf.Max(invincibleTimer, duration);
    }
}
