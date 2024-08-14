using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;

namespace ROGUE.TD
{
    public readonly partial struct Aspect_EnemySpawner : IAspect
    {
        public readonly Entity entity;

        //private readonly RefRW<Component_WorldMap> _enemySpawnerProperties;
        //private readonly RefRW<LocalTransform> localTransform;

        private readonly RefRW<Component_EnemySpawner> _enemySpawner;


        public Entity SkeletonEnemyPrefab => _enemySpawner.ValueRO.prefab;


        public float EnemySpawnTimer
        {
            get => _enemySpawner.ValueRO.nextSpawnTime;
            set => _enemySpawner.ValueRW.nextSpawnTime = value;
        }

        public float EnemySpawnRate => _enemySpawner.ValueRO.spawnRate;

        public float3 spawnPosition => _enemySpawner.ValueRO.spawnPosition;

    }
}
