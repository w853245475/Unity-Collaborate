using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

/// <summary>
/// 该系统用于处理塔与敌人之间的触发检测，当敌人进入塔的范围时，记录该敌人。
/// </summary>
[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct TriggerDetectionSystem : ISystem
{
    // 缓冲区查找器，用于存储敌人列表
    private BufferLookup<EnemyInRangeBuffer> bufferLookup;

    // 组件查找器，用于确认实体是否具有 EnemyComponent
    private ComponentLookup<EnemyComponent> enemyComponentLookup;

    /// <summary>
    /// 在系统创建时调用，初始化 Lookup 对象。
    /// </summary>
    public void OnCreate(ref SystemState state)
    {
        // 确保系统在有 TowerComponent 和 EnemyComponent 存在时运行
        state.RequireForUpdate<TowerComponent>();
        state.RequireForUpdate<EnemyComponent>();

        // 初始化缓冲区和组件查找器
        bufferLookup = state.GetBufferLookup<EnemyInRangeBuffer>();
        enemyComponentLookup = state.GetComponentLookup<EnemyComponent>(isReadOnly: true);
    }

    /// <summary>
    /// 每帧调用，用于更新 Lookup 对象，并执行触发检测作业。
    /// </summary>
    public void OnUpdate(ref SystemState state)
    {
        // 更新缓冲区和组件查找器，使它们在当前帧中保持最新
        bufferLookup.Update(ref state);
        enemyComponentLookup.Update(ref state);

        // 定义触发检测作业
        var triggerJob = new TriggerJob
        {
            BufferFromEntity = bufferLookup,  // 提供缓冲区查找器
            EnemyComponentLookup = enemyComponentLookup // 提供敌人组件查找器
        };

        // 调度触发检测作业，等待物理引擎的结果
        state.Dependency = triggerJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // 明确完成作业，防止系统继续处理之前未完成的任务
        state.Dependency.Complete();
    }

    /// <summary>
    /// 触发检测作业，处理所有物理触发事件。
    /// </summary>
    [BurstCompile]
    struct TriggerJob : ITriggerEventsJob
    {
        // 缓冲区查找器，用于存储检测到的敌人实体
        public BufferLookup<EnemyInRangeBuffer> BufferFromEntity;

        // 组件查找器，用于确认实体是否为敌人
        [ReadOnly] public ComponentLookup<EnemyComponent> EnemyComponentLookup;

        /// <summary>
        /// 处理每个触发事件的逻辑。
        /// </summary>
        /// <param name="triggerEvent">触发事件，包含两个实体。</param>
        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            // 检查触发事件的实体B是否为敌人
            if (entityB != Entity.Null && EnemyComponentLookup.HasComponent(entityB))
            {
                // 如果实体A有 EnemyInRangeBuffer 缓冲区，则将敌人实体添加到该缓冲区
                if (BufferFromEntity.HasBuffer(entityA))
                {
                    var buffer = BufferFromEntity[entityA];
                    buffer.Add(new EnemyInRangeBuffer { EnemyEntity = entityB });
                }
            }
        }
    }
}
