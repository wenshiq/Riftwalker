using System;
using UnityEngine;

/// <summary>
/// 玩家生命：血量、无敌帧、受伤闪烁、死亡、治疗、提升上限、重开一局。
/// </summary>
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("生命")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float hitInvincibleTime = 0.5f;
    [SerializeField] private float flashInterval = 0.08f;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;
    public event Action OnDied;

    private int baseMaxHealth;
    private float invincibleTimer;
    private SpriteRenderer sprite;

    private void Awake()
    {
        baseMaxHealth = maxHealth;
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
        if (invincibleTimer > 0f) return;

        CurrentHealth -= amount;
        Debug.Log("[PlayerHealth] 受伤 " + amount + "，当前血量 " + CurrentHealth + "/" + maxHealth);
        SetInvincible(hitInvincibleTime);

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            OnDied?.Invoke();
            gameObject.SetActive(false);
        }
    }

    /// <summary>回血，不超过上限。</summary>
    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
    }

    /// <summary>提升生命上限，并同步回满这部分血量。</summary>
    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        CurrentHealth += amount;
    }

    /// <summary>设置无敌时间（取较大值），用于受击保护或闪避无敌帧。</summary>
    public void SetInvincible(float duration)
    {
        invincibleTimer = Mathf.Max(invincibleTimer, duration);
    }

    /// <summary>重开一局：恢复初始血量上限并满血，重新激活对象。</summary>
    public void Reset()
    {
        maxHealth = baseMaxHealth;
        CurrentHealth = baseMaxHealth;
        invincibleTimer = 0f;
        gameObject.SetActive(true);
    }
}
