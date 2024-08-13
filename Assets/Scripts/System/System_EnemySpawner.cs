using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

//public struct ZombieSpawnTimer : IComponentData
//{
//    public float Value;
//}


namespace ROGUE.TD
{

    [BurstCompile]
    public partial struct System_EnemySpawner : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            //state.RequireForUpdate<ZombieSpawnTimer>();
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

            //new Job_SpawnEnemy
            //{
            //    deltaTime = deltaTime,
            //    ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged)
            //}.Run();


            foreach (RefRW<Component_EnemySpawner> spawner in SystemAPI.Query<RefRW<Component_EnemySpawner>>())
            {
                ProcessSpawner(ref state, spawner);
            }

        }

        private void ProcessSpawner(ref SystemState state, RefRW<Component_EnemySpawner> spawner)
        {
            if (spawner.ValueRO.nextSpawnTime < SystemAPI.Time.ElapsedTime)
            {
                Entity instantiated = state.EntityManager.Instantiate(spawner.ValueRO.prefab);
                state.EntityManager.SetComponentData(instantiated, LocalTransform.FromPositionRotationScale(spawner.ValueRO.spawnPosition, Quaternion.identity, 5));
                //state.EntityManager.SetComponentData(instantiated, LocalTransform.FromScale(5));
                spawner.ValueRW.nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + spawner.ValueRO.spawnRate;
            }
        }

        [BurstCompile]
        public partial struct Job_SpawnEnemy : IJobEntity
        {
            public float deltaTime;
            public EntityCommandBuffer ECB;


            [BurstCompile]
            private void Execute(Aspect_EnemySpawner enemySpawner)
            {
                //if (enemySpawner.ValueRO.nextSpawnTime < SystemAPI.Time.ElapsedTime)
                //{
                //    Entity instantiated = state.EntityManager.Instantiate(spawner.ValueRO.prefab);
                //    state.EntityManager.SetComponentData(instantiated, LocalTransform.FromPositionRotationScale(spawner.ValueRO.spawnPosition, Quaternion.identity, 5));
                //    //state.EntityManager.SetComponentData(instantiated, LocalTransform.FromScale(5));
                //    spawner.ValueRW.nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + spawner.ValueRO.spawnRate;
                //}


            }
        }
    }
}