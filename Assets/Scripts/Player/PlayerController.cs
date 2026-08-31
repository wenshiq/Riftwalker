using UnityEngine;

/// <summary>
/// 玩家控制器：WASD 八向移动 + 空格闪避（带无敌帧）。
/// 移速从 PlayerStats 读取，方便被强化修改。
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("闪避")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.6f;
    [SerializeField] private float dashInvincibleTime = 0.25f;

    private Rigidbody2D rb;
    private PlayerHealth health;
    private PlayerStats stats;
    private Vector2 moveInput;
    private Vector2 facing = Vector2.right;

    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<PlayerHealth>();
        stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        ReadInput();
        UpdateTimers();
    }

    private void FixedUpdate()
    {
        float speed = stats != null ? stats.moveSpeed : 5f;
        Vector2 velocity = isDashing ? facing * dashSpeed : moveInput * speed;
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    private void ReadInput()
    {
        Vector2 raw = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveInput = raw.normalized;
        if (moveInput != Vector2.zero) facing = moveInput;

        if (Input.GetKeyDown(KeyCode.Space) && dashCooldownTimer <= 0f)
            StartDash();
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        health?.SetInvincible(dashInvincibleTime);
    }

    private void UpdateTimers()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f) isDashing = false;
        }
        if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.deltaTime;
    }
}
