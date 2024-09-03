using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;


public class Authoring_Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    // Count down timer to fire attack when attackTimer == 0;
    public float attackTimer;
    public float attackRange;
    public float attackRate;
    public float attackDamage;
}

public struct Component_EnemyDebuff : IBufferElementData
{
    public DebuffType Type;       // Debuff类型（例如 Burn、Slow、Poison 等）
    public float Duration;        // Debuff的持续时间
    public float Intensity;       // Debuff的强度（例如易伤效果的比例、减速比例等）
}

public struct Component_Enemy : IComponentData
{
    public float speed;
    public float health;

}

public struct Component_EnemyAttackProperties : IComponentData, IEnableableComponent
{
    public float attackRange;
    public float attackTimer;
    public float attackRate;
    public float attackDamage;
}



public class EnemyBaker : Baker<Authoring_Enemy>
{
    public override void Bake(Authoring_Enemy authoring)
    {

        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new Component_Enemy
        {
            speed = authoring.speed,
            health = authoring.health,

        });

        AddComponent(entity, new Component_EnemyAttackProperties
        {
            attackTimer = authoring.attackTimer,
            attackRate = authoring.attackRate,
            attackDamage = authoring.attackDamage,
            attackRange = authoring.attackRange
        });
        // 添加 Component_EnemyDebuff 缓冲区
        AddBuffer<Component_EnemyDebuff>(entity);
    }
}
