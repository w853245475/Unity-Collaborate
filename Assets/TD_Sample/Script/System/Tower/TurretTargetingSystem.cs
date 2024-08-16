using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct TurretTargetingSystem : ISystem
{
    private ComponentLookup<EnemyComponent> enemyComponentLookup;

    /// <summary>
    /// 系统的初始化方法，在系统被创建时调用。
    /// </summary>
    public void OnCreate(ref SystemState state)
    {
        // 确保系统在执行之前需要特定组件的存在。
        state.RequireForUpdate<TowerComponent>();
        state.RequireForUpdate<EnemyComponent>();

        // 初始化 enemyComponentLookup 用于查询敌人组件。
        enemyComponentLookup = state.GetComponentLookup<EnemyComponent>(isReadOnly: true);
    }

    /// <summary>
    /// 系统的更新方法，在每一帧调用。
    /// </summary>
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // 更新敌人组件的查询状态。
        enemyComponentLookup.Update(ref state);

        // 创建 EntityCommandBuffer，用于处理实体的命令（如创建或修改实体）。
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        // 查询所有塔实体并循环处理。
        foreach (var (tower, towerEntity) in SystemAPI.Query<RefRW<TowerComponent>>().WithEntityAccess())
        {
            // 获取塔的变换组件和其范围内的敌人缓冲区。
            var transform = state.EntityManager.GetComponentData<LocalTransform>(towerEntity);
            var enemyBuffer = state.EntityManager.GetBuffer<EnemyInRangeBuffer>(towerEntity);

            // 如果没有敌人在范围内，跳过此塔。
            if (enemyBuffer.Length == 0)
                continue;

            // 查找离塔最近的敌人。
            if (FindClosestEnemy(ref state, towerEntity, enemyBuffer, transform.Position, out float3 closestEnemyPosition))
            {
                // 使塔旋转朝向最近的敌人。
                RotateTurretTowardsEnemy(ref tower.ValueRW, ref transform, closestEnemyPosition, deltaTime);

                // 尝试向敌人射击。
                TryShootAtEnemy(ref state, ref tower.ValueRW, transform, closestEnemyPosition, ref ecb, deltaTime);
            }

            // 更新塔的组件数据以应用变更。
            state.EntityManager.SetComponentData(towerEntity, tower.ValueRW);
            state.EntityManager.SetComponentData(towerEntity, transform);
        }

        // 执行所有缓冲的实体命令。
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    /// <summary>
    /// 查找最近的敌人。
    /// </summary>
    /// <param name="state">系统状态</param>
    /// <param name="towerEntity">塔实体</param>
    /// <param name="enemyBuffer">塔范围内的敌人缓冲区</param>
    /// <param name="towerPosition">塔的位置</param>
    /// <param name="closestEnemyPosition">输出的最近敌人的位置</param>
    /// <returns>是否找到敌人</returns>
    private bool FindClosestEnemy(ref SystemState state, Entity towerEntity, DynamicBuffer<EnemyInRangeBuffer> enemyBuffer, float3 towerPosition, out float3 closestEnemyPosition)
    {
        closestEnemyPosition = float3.zero;
        float closestDistance = float.MaxValue;
        bool foundEnemy = false;

        // 遍历敌人缓冲区中的每一个敌人，找到离塔最近的敌人。
        foreach (var enemyBufferElement in enemyBuffer)
        {
            var enemyEntity = enemyBufferElement.EnemyEntity;

            // 确保敌人实体具有 LocalToWorld 和 EnemyComponent 组件。
            if (!state.EntityManager.HasComponent<LocalToWorld>(enemyEntity) || !enemyComponentLookup.HasComponent(enemyEntity))
                continue;

            // 获取敌人的位置并计算与塔的距离。
            var enemyPosition = state.EntityManager.GetComponentData<LocalToWorld>(enemyEntity).Position;
            float distance = math.distance(towerPosition, enemyPosition);

            // 如果当前敌人距离比之前记录的最近敌人更近，更新最近的敌人。
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemyPosition = enemyPosition;
                foundEnemy = true;
            }
        }

        return foundEnemy;
    }

    /// <summary>
    /// 使塔旋转以朝向最近的敌人。
    /// </summary>
    /// <param name="tower">塔组件</param>
    /// <param name="transform">塔的变换组件</param>
    /// <param name="closestEnemyPosition">最近的敌人位置</param>
    /// <param name="deltaTime">时间增量</param>
    private void RotateTurretTowardsEnemy(ref TowerComponent tower, ref LocalTransform transform, float3 closestEnemyPosition, float deltaTime)
    {
        // 计算塔到敌人的方向向量并生成目标旋转。
        float3 direction = math.normalize(closestEnemyPosition - transform.Position);
        quaternion targetRotation = quaternion.LookRotationSafe(direction, math.up());

        // 使用线性插值平滑塔的旋转，以逐渐朝向目标旋转。
        transform.Rotation = math.slerp(transform.Rotation, targetRotation, deltaTime * tower.RotationSpeed);
    }

    /// <summary>
    /// 向敌人发射子弹。
    /// </summary>
    /// <param name="state">系统状态</param>
    /// <param name="tower">塔组件</param>
    /// <param name="transform">塔的变换组件</param>
    /// <param name="closestEnemyPosition">最近的敌人位置</param>
    /// <param name="ecb">实体命令缓冲</param>
    /// <param name="deltaTime">时间增量</param>
    private void TryShootAtEnemy(ref SystemState state, ref TowerComponent tower, LocalTransform transform, float3 closestEnemyPosition, ref EntityCommandBuffer ecb, float deltaTime)
    {
        // 计算塔到敌人的方向向量。
        float3 direction = math.normalize(closestEnemyPosition - transform.Position);

        // 更新塔的射击计时器。
        tower.TimeSinceLastShot += deltaTime;

        // 如果塔的射击计时器超过了其射速，则进行射击。
        if (tower.TimeSinceLastShot >= tower.fireRate)
        {
            tower.TimeSinceLastShot = 0;
            tower.TargetDirection = direction;

            // 确保塔具有有效的发射点。
            if (!state.EntityManager.HasComponent<LocalToWorld>(tower.SpawnPoint))
            {
                Debug.LogWarning("Tower does not have a valid SpawnPoint.");
                return;
            }

            // 获取发射点的位置并实例化子弹。
            float3 spawnPosition = state.EntityManager.GetComponentData<LocalToWorld>(tower.SpawnPoint).Position;
            Entity bulletInstance = ecb.Instantiate(tower.BulletPrefab);
            quaternion rotation = quaternion.LookRotationSafe(tower.TargetDirection, math.up());

            // 设置子弹的初始位置、旋转和缩放。
            ecb.SetComponent(bulletInstance, new LocalTransform
            {
                Position = spawnPosition,
                Rotation = rotation,
                Scale = state.EntityManager.GetComponentData<LocalTransform>(tower.BulletPrefab).Scale
            });

            // 设置子弹的速度和方向。
            var bulletComponent = state.EntityManager.GetComponentData<BulletComponent>(tower.BulletPrefab);
            ecb.SetComponent(bulletInstance, new BulletComponent
            {
                Direction = tower.TargetDirection,
                Speed = bulletComponent.Speed
            });
        }
    }
}
