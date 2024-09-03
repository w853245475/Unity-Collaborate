using Unity.Burst;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Physics;
using Unity.Collections;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct System_BulletCollision : ISystem
{
    // Lookup tables 用于获取 BulletComponent 和 EnemyComponent 的只读组件数据
    private ComponentLookup<Component_Bullet> bulletLookup;
    private ComponentLookup<Component_Enemy> enemyLookup;
    private BufferLookup<Component_EnemyDebuff> enemyDebuffBufferLookup;

    /// <summary>
    /// 系统创建时调用，初始化组件查找器
    /// </summary>
    public void OnCreate(ref SystemState state)
    {
        // 要求系统更新的前提条件是必须存在 BulletComponent 和 EnemyComponent
        state.RequireForUpdate<Component_Bullet>();
        state.RequireForUpdate<Component_Enemy>();

        // 初始化 ComponentLookup，用于高效查询组件数据
        bulletLookup = state.GetComponentLookup<Component_Bullet>(true);
        enemyLookup = state.GetComponentLookup<Component_Enemy>(true);
        enemyDebuffBufferLookup = state.GetBufferLookup<Component_EnemyDebuff>(false);
    }

    /// <summary>
    /// 系统更新时调用，刷新组件查找器并安排物理触发检测任务
    /// </summary>
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 在每帧更新时刷新 ComponentLookup 和 BufferLookup，以确保其使用的是最新的数据
        bulletLookup.Update(ref state);
        enemyLookup.Update(ref state);
        enemyDebuffBufferLookup.Update(ref state);

        // 获取 EndSimulationEntityCommandBufferSystem 的 Singleton，创建命令缓冲区
        var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

        // 定义并行的 TriggerJob 作业，用于处理物理触发事件
        var job = new TriggerJob
        {
            Ecb = ecb.AsParallelWriter(),
            BulletLookup = bulletLookup,
            EnemyLookup = enemyLookup,
            EnemyDebuffBufferLookup = enemyDebuffBufferLookup
        };

        // 调度并行作业，处理物理触发事件
        state.Dependency = job.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    /// <summary>
    /// 并行作业用于处理物理触发事件，检查子弹和敌人的碰撞并应用相应的逻辑（如减少生命值、应用Debuff和销毁子弹）。
    /// </summary>
    [BurstCompile]
    struct TriggerJob : ITriggerEventsJob
    {
        // 并行的命令缓冲区，用于安全地记录实体操作（如销毁或设置组件数据）
        public EntityCommandBuffer.ParallelWriter Ecb;

        // Lookup tables 用于访问子弹和敌人组件的只读数据
        [ReadOnly] public ComponentLookup<Component_Bullet> BulletLookup;
        [ReadOnly] public ComponentLookup<Component_Enemy> EnemyLookup;

        // BufferLookup 用于访问和修改敌人的Debuff缓冲区
        public BufferLookup<Component_EnemyDebuff> EnemyDebuffBufferLookup;

        /// <summary>
        /// 处理触发事件，每个触发事件调用一次
        /// </summary>
        public void Execute(TriggerEvent triggerEvent)
        {
            // 获取触发事件中的两个实体
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            // 检查实体A是否为子弹，实体B是否为敌人
            if (BulletLookup.HasComponent(entityA) && EnemyLookup.HasComponent(entityB))
            {
                ApplyEffects(entityA, entityB, 0);
            }
            // 检查实体B是否为子弹，实体A是否为敌人
            else if (BulletLookup.HasComponent(entityB) && EnemyLookup.HasComponent(entityA))
            {
                ApplyEffects(entityB, entityA, 1);
            }
        }

        /// <summary>
        /// 处理子弹碰撞的效果，应用Debuff并减少敌人的生命值
        /// </summary>
        private void ApplyEffects(Entity bulletEntity, Entity enemyEntity, int jobIndex)
        {
            var bullet = BulletLookup[bulletEntity];
            var enemy = EnemyLookup[enemyEntity];

            // 调试信息以确保值正确传递
           // Debug.Log($"Bullet Collision: DebuffType={bullet.DebuffType}, Duration={bullet.DebuffDuration}, Intensity={bullet.DebuffIntensity}");

            // 如果子弹携带Debuff，则将其应用到敌人身上
            if (bullet.DebuffType != DebuffType.None)
            {
                var debuffBuffer = EnemyDebuffBufferLookup[enemyEntity];
                debuffBuffer.Add(new Component_EnemyDebuff
                {
                    Type = bullet.DebuffType,
                    Duration = bullet.DebuffDuration,
                    Intensity = bullet.DebuffIntensity
                });

                //Debug.Log($"Debuff {bullet.DebuffType} added to enemy with duration {bullet.DebuffDuration} and intensity {bullet.DebuffIntensity}");
            }

            // 减少敌人的生命值并销毁子弹实体
            enemy.health -= 10f;
            Ecb.SetComponent(jobIndex, enemyEntity, enemy);
            Ecb.DestroyEntity(jobIndex, bulletEntity);
        }
    }
}
