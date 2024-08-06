using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class Spawner_Enemy_Authoring : MonoBehaviour
{
    public GameObject prefab;
    public GameObject target;
    public float spawnRate;
}

public struct Spawner_Enemy : IComponentData
{
    public Entity prefab;
    public Entity target;
    public float3 spawnPosition;
    public float nextSpawnTime;
    public float spawnRate;
}

public class Spawner_Enemy_Baker : Baker<Spawner_Enemy_Authoring>
{
    public override void Bake(Spawner_Enemy_Authoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new Spawner_Enemy
        {
            prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),

            // target = GetEntity(authoring.target, TransformUsageFlags.None),
            spawnPosition = authoring.transform.position,
            nextSpawnTime = 0.0f,
            spawnRate = authoring.spawnRate,
        });
    }
}