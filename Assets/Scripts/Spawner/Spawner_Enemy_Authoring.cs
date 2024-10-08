using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ROGUE.TD
{
    public class Spawner_Enemy_Authoring : MonoBehaviour
    {
        public GameObject prefab;
        public float spawnRate;
    }

    public struct Component_EnemySpawner : IComponentData
    {
        public Entity prefab;
        public float3 spawnPosition;
        public float nextSpawnTime;
        public float spawnRate;
    }

    public class Spawner_Enemy_Baker : Baker<Spawner_Enemy_Authoring>
    {
        public override void Bake(Spawner_Enemy_Authoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Component_EnemySpawner
            {
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),

                spawnPosition = authoring.transform.position,
                nextSpawnTime = 3.0f,
                spawnRate = authoring.spawnRate,
            });
        }
    }
}