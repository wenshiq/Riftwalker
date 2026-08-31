using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 总控：管理一局流程（战斗中 → 选强化 → 下一房间）。
/// M2 只做基础循环；BOSS / 通关 / 死亡结算放 M3。
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private List<UpgradeData> upgradePool = new List<UpgradeData>();

    private RoomManager roomManager;
    private UpgradeUI upgradeUI;
    private PlayerStats playerStats;

    private void Awake()
    {
        Instance = this;
        roomManager = GetComponent<RoomManager>();
        upgradeUI = GetComponent<UpgradeUI>();
    }

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            playerStats = p.GetComponent<PlayerStats>();
            PlayerHealth health = p.GetComponent<PlayerHealth>();
            if (health != null) health.OnDied += HandlePlayerDied;
        }

        roomManager.OnRoomCleared += HandleRoomCleared;
        roomManager.StartRun();
    }

    public void SetUpgradePool(List<UpgradeData> pool) => upgradePool = pool;

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

    private void HandlePlayerDied()
    {
        Debug.Log("玩家死亡（M2 先简单处理，M3 做死亡结算/重开）");
        Time.timeScale = 0f;
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
