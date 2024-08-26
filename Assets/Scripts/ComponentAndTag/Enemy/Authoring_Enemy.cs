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
    }
}
