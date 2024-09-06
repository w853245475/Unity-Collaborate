using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ROGUE.TD
{

    [BurstCompile]
    public partial struct System_EnemySpawner : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            float3 randomDirection = UnityEngine.Random.onUnitSphere; // Get a random direction on a sphere
            float3 randomPosition = randomDirection * 40; // Scale the direction by the distance

            new Job_SpawnEnemy
            {
                deltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged),
                spawnPosition = new float3(randomPosition.x, 0.8f, randomPosition.z)
            }.Run();

        }

        [BurstCompile]
        public partial struct Job_SpawnEnemy : IJobEntity
        {
            public float deltaTime;
            public EntityCommandBuffer ECB;
            public float3 spawnPosition;

            [BurstDiscard]
            private void DebugInfo()
            {
                Debug.Log("<b> <size=13> <color=#9DF155>Info : 3 SetDataSystem : Setting Data .</color> </size> </b>");
            }

            [BurstCompile]
            private void Execute(Aspect_EnemySpawner enemySpawner)
            {
                enemySpawner.EnemySpawnTimer -= deltaTime;

                if (enemySpawner.EnemySpawnTimer > 0) return;

                DebugInfo();

                var newZombie = ECB.Instantiate(enemySpawner.SkeletonEnemyPrefab);
                ECB.SetComponent(newZombie, LocalTransform.FromPositionRotationScale(spawnPosition, Quaternion.identity, 5));
                enemySpawner.EnemySpawnTimer = enemySpawner.EnemySpawnRate;
            }
        }
    }
}