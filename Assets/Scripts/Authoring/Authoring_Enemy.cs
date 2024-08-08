using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;


public class Authoring_Enemy : MonoBehaviour
{
    public float speed;
    public float health;
}


public struct Component_Enemy : IComponentData
{
    public float speed;
    public float health;
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
        }); ;
    }
}
