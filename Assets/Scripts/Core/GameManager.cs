using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 总控：管理一局流程（战斗中 → 选强化 → 下一房间 → 死亡/通关）。
/// 强化数据在 Awake 里用 Resources.LoadAll 从 Assets/Resources/Data 加载。
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private List<UpgradeData> upgradePool = new List<UpgradeData>();

    private RoomManager roomManager;
    private UpgradeUI upgradeUI;
    private EndScreenUI endScreenUI;
    private PlayerStats playerStats;
    private PlayerHealth playerHealth;
    private Transform playerTransform;

    private void Awake()
    {
        Instance = this;
        roomManager = GetComponent<RoomManager>();
        upgradeUI = GetComponent<UpgradeUI>();
        endScreenUI = GetComponent<EndScreenUI>();

        upgradePool = new List<UpgradeData>(Resources.LoadAll<UpgradeData>("Data"));
        if (upgradePool.Count == 0)
            Debug.LogWarning("GameManager：没有加载到强化数据，请先运行菜单 Riftwalker → 一键搭建 M2 场景。");
    }

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            playerTransform = p.transform;
            playerStats = p.GetComponent<PlayerStats>();
            playerHealth = p.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.OnDied += HandlePlayerDied;
        }

        roomManager.OnRoomCleared += HandleRoomCleared;
        roomManager.OnBossDefeated += HandleBossDefeated;
        roomManager.StartRun();
    }

    private void HandleRoomCleared()
    {
        Time.timeScale = 0f; // 暂停游戏，弹出强化
        List<UpgradeData> choices = PickRandomUpgrades(3);
        upgradeUI.Show(choices, OnUpgradeChosen);
    }

    private void OnUpgradeChosen(UpgradeData upgrade)
    {
        if (playerStats != null) playerStats.ApplyUpgrade(upgrade);

        Time.timeScale = 1f;
        roomManager.StartNextRoom();
    }

    private void HandleBossDefeated()
    {
        Time.timeScale = 0f;
        endScreenUI.Show("通关了！", Restart);
    }

    private void HandlePlayerDied()
    {
        Time.timeScale = 0f;
        endScreenUI.Show("你死了", Restart);
    }

    /// <summary>重开一局：重置玩家属性/位置/血量，清空敌人，从第 1 房间重新开始。</summary>
    private void Restart()
    {
        Time.timeScale = 1f;
        if (playerStats != null) playerStats.Reset();
        if (playerHealth != null) playerHealth.Reset();
        if (playerTransform != null) playerTransform.position = Vector3.zero;
        roomManager.ResetRun();
        roomManager.StartRun();
    }

    private List<UpgradeData> PickRandomUpgrades(int count)
    {
        List<UpgradeData> pool = new List<UpgradeData>(upgradePool);
        List<UpgradeData> result = new List<UpgradeData>();
        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            result.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
        return result;
    }
}
