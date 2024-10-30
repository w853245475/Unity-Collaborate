using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;



namespace ROGUE.TD
{
    [BurstCompile]
    // [UpdateAfter(typeof(System_EnemyMove))]
    public partial struct System_EnemyAttack : ISystem
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

            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton; 

            SystemAPI.TryGetSingleton(out ecbSingleton);

            var player = SystemAPI.GetSingletonEntity<Component_PlayerBase>();
            var playerPos = SystemAPI.GetComponent<LocalTransform>(player).Position;

            new ZombieEatJob
            {
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                PlayerEntity = player,
                PlayerPos = playerPos
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct ZombieEatJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        public Entity PlayerEntity;
        public float3 PlayerPos;

        [BurstCompile]
        private void Execute(Aspect_EnemyAttack enemyAttack, [EntityIndexInQuery] int sortKey)
        {
            if (enemyAttack.IsInAttackRange(PlayerPos))
            {
                enemyAttack.Attack(DeltaTime, ECB, sortKey, PlayerEntity);
            }
            //else
            //{
            //    ECB.SetComponentEnabled<ZombieEatProperties>(sortKey, zombie.Entity, false);
            //    ECB.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, true);
            //}
        }
    }
}