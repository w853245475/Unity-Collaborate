using Unity.Burst;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Physics;
using Unity.Collections;

/// <summary>
/// 系统用于检测子弹与敌人之间的碰撞，并处理碰撞结果（如减少敌人生命值和销毁子弹）。
/// </summary>
[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct BulletTriggerSystem : ISystem
{
    // Lookup tables 用于获取 BulletComponent 和 EnemyComponent 的只读组件数据
    private ComponentLookup<BulletComponent> bulletLookup;
    private ComponentLookup<EnemyComponent> enemyLookup;

    /// <summary>
    /// 系统创建时调用，初始化组件查找器
    /// </summary>
    public void OnCreate(ref SystemState state)
    {
        // 要求系统更新的前提条件是必须存在 BulletComponent 和 EnemyComponent
        state.RequireForUpdate<BulletComponent>();
        state.RequireForUpdate<EnemyComponent>();

        // 初始化 ComponentLookup，用于高效查询组件数据
        bulletLookup = state.GetComponentLookup<BulletComponent>(true);
        enemyLookup = state.GetComponentLookup<EnemyComponent>(true);
    }

    /// <summary>
    /// 系统更新时调用，刷新组件查找器并安排物理触发检测任务
    /// </summary>
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 在每帧更新时刷新 ComponentLookup，以确保其使用的是最新的数据
        bulletLookup.Update(ref state);
        enemyLookup.Update(ref state);

        // 获取 EndSimulationEntityCommandBufferSystem 的 Singleton，创建命令缓冲区
        var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

        // 定义并行的 TriggerJob 作业，用于处理物理触发事件
        var job = new TriggerJob
        {
            Ecb = ecb.AsParallelWriter(),
            BulletLookup = bulletLookup,
            EnemyLookup = enemyLookup
        };

        // 调度并行作业，处理物理触发事件
        state.Dependency = job.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    /// <summary>
    /// 并行作业用于处理物理触发事件，检查子弹和敌人的碰撞并应用相应的逻辑（如减少生命值和销毁子弹）。
    /// </summary>
    [BurstCompile]
    struct TriggerJob : ITriggerEventsJob
    {
        // 并行的命令缓冲区，用于安全地记录实体操作（如销毁或设置组件数据）
        public EntityCommandBuffer.ParallelWriter Ecb;

        // Lookup tables 用于访问子弹和敌人组件的只读数据
        [ReadOnly] public ComponentLookup<BulletComponent> BulletLookup;
        [ReadOnly] public ComponentLookup<EnemyComponent> EnemyLookup;

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
                // 更新敌人生命值
                var enemy = EnemyLookup[entityB];
                enemy.Health -= 10f;

                // 应用敌人组件的修改，并销毁子弹
                Ecb.SetComponent(0, entityB, enemy);
                Ecb.DestroyEntity(0, entityA);
            }
            // 检查实体B是否为子弹，实体A是否为敌人
            else if (BulletLookup.HasComponent(entityB) && EnemyLookup.HasComponent(entityA))
            {
                // 更新敌人生命值
                var enemy = EnemyLookup[entityA];
                enemy.Health -= 10f;

                // 应用敌人组件的修改，并销毁子弹
                Ecb.SetComponent(0, entityA, enemy);
                Ecb.DestroyEntity(0, entityB);
            }
        }
    }
}
