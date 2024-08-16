using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

/// <summary>
/// 系统负责检查敌人实体的生命值，并销毁生命值为 0 或更低的敌人。
/// </summary>
[BurstCompile]
public partial struct EnemyDestroySystem : ISystem
{
    // 系统在创建时执行的初始化操作
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // 确保只有包含 EnemyComponent 的实体才会触发该系统的更新
        state.RequireForUpdate<EnemyComponent>();
    }

    // 系统在每帧更新时执行的逻辑
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 使用 EntityCommandBuffer 来记录销毁实体的操作
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var ecbParallel = ecb.AsParallelWriter();

        // 定义并行任务
        var jobHandle = new EnemyDestroyJob
        {
            ECB = ecbParallel
        }.ScheduleParallel(state.Dependency);

        // 需要显式调用 Complete 来确保任务完成
        jobHandle.Complete();

        // 回放命令缓冲区，应用所有实体销毁操作
        ecb.Playback(state.EntityManager);

        // 释放命令缓冲区
        ecb.Dispose();
    }

    /// <summary>
    /// 并行任务：检查敌人实体的生命值，并在其小于等于 0 时销毁实体。
    /// </summary>
    [BurstCompile]
    public partial struct EnemyDestroyJob : IJobEntity
    {
        // 用于记录销毁实体的命令缓冲区，支持并行写入
        public EntityCommandBuffer.ParallelWriter ECB;

        /// <summary>
        /// 执行敌人销毁检查
        /// </summary>
        /// <param name="entity">当前处理的敌人实体</param>
        /// <param name="entityInQueryIndex">实体在查询中的索引，用于并行写入的正确性</param>
        /// <param name="enemyComponent">敌人的组件数据</param>
        public void Execute(Entity entity, [EntityIndexInQuery] int entityInQueryIndex, ref EnemyComponent enemyComponent)
        {
            // 如果敌人的生命值小于或等于 0，则销毁该敌人实体
            if (enemyComponent.Health <= 0)
            {
                ECB.DestroyEntity(entityInQueryIndex, entity);
            }
        }
    }
}
