public enum DebuffType
{
    None = 0,      // 无Debuff
    Burn,      // 易伤Debuff：受到的伤害增加
    Slow,      // 减速Debuff：移动速度减少
    Poison,    // 中毒Debuff：每秒受到持续伤害
    ChainLightning // 闪电链Debuff：伤害传递
    // 其他元素类型可以扩展更多Debuff类型
}
