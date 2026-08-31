using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 房间/波次管理：按房间刷怪、追踪存活敌人、清空后通知进入强化阶段。
/// M2 的"房间"= 一波敌人（真实墙体房间 + 小地图放 M3）。
/// 敌人数据在 Awake 里用 Resources.LoadAll 从 Assets/Resources/Data 加载。
/// </summary>
public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [Header("波次配置")]
    [SerializeField] private int baseEnemyCount = 3;
    [SerializeField] private int enemyCountPerRoom = 2;
    [SerializeField] private float spawnMinRadius = 3f;
    [SerializeField] private float spawnMaxRadius = 6f;

    private List<EnemyData> enemyTypes = new List<EnemyData>();
    private readonly List<Enemy> alive = new List<Enemy>();

    public int CurrentRoom { get; private set; }
    public event System.Action OnRoomCleared;

    private void Awake()
    {
        Instance = this;

        enemyTypes = new List<EnemyData>(Resources.LoadAll<EnemyData>("Data"));
        if (enemyTypes.Count == 0)
            Debug.LogError("RoomManager：没有加载到任何敌人数据，请先运行菜单 Riftwalker → 一键搭建 M2 场景。");
    }

    public void StartRun()
    {
        CurrentRoom = 0;
        StartNextRoom();
    }

    public void StartNextRoom()
    {
        CurrentRoom++;
        SpawnRoom(CurrentRoom);
    }

    /// <summary>重开一局：清空场上所有敌人并重置房间计数。</summary>
    public void ResetRun()
    {
        foreach (Enemy e in alive)
        {
            if (e != null) Destroy(e.gameObject);
        }
        alive.Clear();
        CurrentRoom = 0;
    }

    private void SpawnRoom(int roomNumber)
    {
        int count = baseEnemyCount + (roomNumber - 1) * enemyCountPerRoom;
        for (int i = 0; i < count; i++)
            SpawnEnemy(RandomEnemyType());
    }

    private EnemyData RandomEnemyType()
    {
        if (enemyTypes.Count == 0) return null;
        return enemyTypes[Random.Range(0, enemyTypes.Count)];
    }

    private void SpawnEnemy(EnemyData data)
    {
        if (data == null) return;

        Vector2 center = FindPlayer();
        Vector2 pos = center + Random.insideUnitCircle.normalized * Random.Range(spawnMinRadius, spawnMaxRadius);

        GameObject go = new GameObject("Enemy_" + data.displayName);
        go.transform.position = pos;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 0;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.radius = 0.4f;

        Enemy enemy = go.AddComponent<Enemy>();
        enemy.Init(data);
        alive.Add(enemy);
    }

    private Vector2 FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        return p != null ? (Vector2)p.transform.position : Vector2.zero;
    }

    public void NotifyEnemyDied(Enemy enemy)
    {
        alive.Remove(enemy);
        if (alive.Count == 0)
            OnRoomCleared?.Invoke();
    }
}
