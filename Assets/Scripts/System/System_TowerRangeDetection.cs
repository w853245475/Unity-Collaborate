using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct System_TowerRangeDetection : ISystem
{
    // 缓冲区查找器，用于存储敌人列表
    private BufferLookup<Component_EnemyDetection> bufferLookup;

    // 组件查找器，用于确认实体是否具有 EnemyComponent
    private ComponentLookup<Component_Enemy> enemyComponentLookup;

    public void OnCreate(ref SystemState state)
    {
        // 确保系统在有 TowerComponent 和 EnemyComponent 存在时运行
        state.RequireForUpdate<Component_Tower>();
        state.RequireForUpdate<Component_Enemy>();

        // 初始化缓冲区和组件查找器
        bufferLookup = state.GetBufferLookup<Component_EnemyDetection>();
        enemyComponentLookup = state.GetComponentLookup<Component_Enemy>(isReadOnly: true);
    }

    [BurstCompile]
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
    }

    /// <summary>
    /// 触发检测作业，处理所有物理触发事件。
    /// </summary>
    [BurstCompile]
    struct TriggerJob : ITriggerEventsJob
    {
        // 缓冲区查找器，用于存储检测到的敌人实体
        public BufferLookup<Component_EnemyDetection> BufferFromEntity;

        // 组件查找器，用于确认实体是否为敌人
        [ReadOnly] public ComponentLookup<Component_Enemy> EnemyComponentLookup;

        /// <summary>
        /// 处理每个触发事件的逻辑。
        /// </summary>
        /// <param name="triggerEvent">触发事件，包含两个实体。</param>
        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            // 检查触发事件的实体B是否为敌人
            if (EnemyComponentLookup.HasComponent(entityB) && BufferFromEntity.HasBuffer(entityA))
            {
                var buffer = BufferFromEntity[entityA];
                buffer.Add(new Component_EnemyDetection { EnemyEntity = entityB });
            }
            else if (EnemyComponentLookup.HasComponent(entityA) && BufferFromEntity.HasBuffer(entityB))
            {
                var buffer = BufferFromEntity[entityB];
                buffer.Add(new Component_EnemyDetection { EnemyEntity = entityA });
            }
        }
    }
}
