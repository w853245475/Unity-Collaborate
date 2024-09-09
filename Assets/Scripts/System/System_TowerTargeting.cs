using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
public partial struct System_TowerTargeting : ISystem
{
    private ComponentLookup<Component_Enemy> enemyComponentLookup;

    /// <summary>
    /// 系统的初始化方法，在系统被创建时调用。
    /// </summary>
    public void OnCreate(ref SystemState state)
    {
        // 确保系统在执行之前需要特定组件的存在。
        state.RequireForUpdate<Component_Tower>();
        state.RequireForUpdate<Component_Enemy>();

        // 初始化 enemyComponentLookup 用于查询敌人组件。
        enemyComponentLookup = state.GetComponentLookup<Component_Enemy>(isReadOnly: true);

    }
    /// <summary>
    /// 系统的更新方法，在每一帧调用。
    /// </summary>
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //Debug.Log("System_TowerTargeting Update started.");

        float deltaTime = SystemAPI.Time.DeltaTime;
        enemyComponentLookup.Update(ref state);
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        foreach (var (tower, towerEntity) in SystemAPI.Query<RefRW<Component_Tower>>().WithEntityAccess())
        {
            var transform = state.EntityManager.GetComponentData<LocalTransform>(towerEntity);
            var enemyBuffer = state.EntityManager.GetBuffer<Component_EnemyDetection>(towerEntity);

            if (enemyBuffer.Length == 0)
            {
                //Debug.Log("No enemies in range.");
                continue;
            }

            if (FindClosestEnemy(ref state, towerEntity, enemyBuffer, transform.Position, out float3 closestEnemyPosition))
            {
                TryShootAtEnemy(ref state, ref tower.ValueRW, transform, closestEnemyPosition, ref ecb, towerEntity, deltaTime);
            }

            state.EntityManager.SetComponentData(towerEntity, tower.ValueRW);
            state.EntityManager.SetComponentData(towerEntity, transform);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();

        //Debug.Log("System_TowerTargeting Update ended.");
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
    private bool FindClosestEnemy(ref SystemState state, Entity towerEntity, DynamicBuffer<Component_EnemyDetection> enemyBuffer, float3 towerPosition, out float3 closestEnemyPosition)
    {
        closestEnemyPosition = float3.zero;
        float closestDistance = float.MaxValue;
        bool foundEnemy = false;

        foreach (var enemyBufferElement in enemyBuffer)
        {
            var enemyEntity = enemyBufferElement.EnemyEntity;

            if (!state.EntityManager.HasComponent<LocalToWorld>(enemyEntity) || !enemyComponentLookup.HasComponent(enemyEntity))
                continue;

            var enemyPosition = state.EntityManager.GetComponentData<LocalToWorld>(enemyEntity).Position;
            float distance = math.distance(towerPosition, enemyPosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemyPosition = enemyPosition;
                foundEnemy = true;
                //Debug.Log($"Found enemy at distance {distance}.");
            }
        }

        if (!foundEnemy)
        {
            //Debug.LogWarning("No enemy found.");
        }

        return foundEnemy;
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
  private void TryShootAtEnemy(ref SystemState state, ref Component_Tower tower, LocalTransform transform, float3 closestEnemyPosition, ref EntityCommandBuffer ecb, Entity towerEntity, float deltaTime)
{
    float3 direction = math.normalize(closestEnemyPosition - transform.Position);

    tower.TimeSinceLastShot += deltaTime;

    if (tower.TimeSinceLastShot >= tower.FireRate)
    {
        tower.TimeSinceLastShot = 0;
        tower.TargetDirection = direction;

        //Debug.Log($"Shooting at direction: {direction}");


        var bulletData = state.EntityManager.GetComponentData<Component_BulletData>(towerEntity);
        // 确保发射点存在
        if (!state.EntityManager.HasComponent<LocalToWorld>(bulletData.SpawnPoint))
        {
            //Debug.LogWarning("Tower does not have a valid SpawnPoint.");
            return;
        }
        float3 spawnPosition = state.EntityManager.GetComponentData<LocalToWorld>(bulletData.SpawnPoint).Position;
        // 实例化子弹
        Entity bulletInstance = ecb.Instantiate(bulletData.BulletPrefab);


            quaternion rotation = quaternion.LookRotationSafe(tower.TargetDirection, math.up());
            Debug.Log("Bullet instantiated.");
            Debug.Log(rotation);
            // 设置子弹的组件
            ecb.SetComponent(bulletInstance, new LocalTransform
        {
            Position = spawnPosition,
            Rotation = new quaternion(0, 0, rotation.value.z, 0),
            Scale = state.EntityManager.GetComponentData<LocalTransform>(bulletData.BulletPrefab).Scale
        });

        // 设置子弹的方向和速度
        var bulletComponent = state.EntityManager.GetComponentData<Component_Bullet>(bulletData.BulletPrefab);
        ecb.SetComponent(bulletInstance, new Component_Bullet
        {
            Direction = tower.TargetDirection,
            Speed = bulletComponent.Speed,
            DebuffType = bulletComponent.DebuffType,        // 设置 Debuff 类型
            DebuffDuration = bulletComponent.DebuffDuration, // 设置 Debuff 持续时间
            DebuffIntensity = bulletComponent.DebuffIntensity // 设置 Debuff 强度

        });

        //Debug.Log("Bullet instantiated.");
    }
}


}
