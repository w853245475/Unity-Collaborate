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

    public float attackRate;
}


public struct Component_Enemy : IComponentData
{
    public float speed;
    public float health;
    public float attackTimer;

    public float attackRate;
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
            attackTimer = authoring.attackTimer,
            attackRate = authoring.attackRate
        });
    }
}
