using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;


public class EnemyAuthoring : MonoBehaviour
{
    public float speed;
    public GameObject target;
    public float health;
}


public class EnemyBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {

        Entity entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new Enemy
        {
            speed = authoring.speed,
            target = GetEntity(authoring.target, TransformUsageFlags.Dynamic),
            health = authoring.health,
        }); ;
    }
}
