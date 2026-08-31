using UnityEngine;

/// <summary>强化类型。</summary>
public enum UpgradeType
{
    MoveSpeed,
    FireRate,
    Damage,
    MaxHealth,
    MultiShot
}

/// <summary>
/// 强化项数据（ScriptableObject）。value 是加成数值。
/// </summary>
[CreateAssetMenu(fileName = "UpgradeData", menuName = "Riftwalker/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName = "强化";
    public string description = "";
    public UpgradeType type;
    public float value = 1f;
}
