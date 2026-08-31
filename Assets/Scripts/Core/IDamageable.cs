/// <summary>
/// 可受伤实体的统一接口，玩家和敌人都实现它。
/// 好处：子弹只需要知道「目标实现了 IDamageable」，不用关心对方具体是谁。
/// </summary>
public interface IDamageable
{
    void TakeDamage(int amount);
}
