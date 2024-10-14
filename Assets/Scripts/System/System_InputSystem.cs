using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;


[CreateAfter(typeof(BeginSimulationEntityCommandBufferSystem))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial struct System_InputSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Towers>(); // 确保系统只在 Towers 存在时更新
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        // 在系统销毁时可以清理资源，暂时不需要实现
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 尝试获取 Towers 实体缓冲区
        if (!SystemAPI.TryGetSingletonBuffer<Towers>(out var towers))
        {
            //Debug.LogWarning("No Towers singleton found!");
            return; // 如果没有找到 Towers 单例，则跳过本帧更新
        }

        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var ecbBOS = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        // 获取输入的 TowerPlacementInput 缓冲区
        foreach (var input in SystemAPI.Query<DynamicBuffer<TowerPlacementInput>>())
        {
            foreach (var placementInput in input)
            {
                if (physicsWorld.CastRay(placementInput.Value, out var hit))
                {
                    //Debug.Log($"Raycast hit at: {hit.Position}");

                    // 计算塔应该放置的位置，使用 hit.Position，并适当增加 Y 轴偏移
                    var towerHeightOffset = 0.5f;  // 根据塔的实际高度进行调整
                    var towerPosition = hit.Position + new float3(0, towerHeightOffset, 0);
                    //Debug.Log($"Calculated tower position: {towerPosition}");

                    // 使用 NativeList 来存储检测到的碰撞
                    NativeList<DistanceHit> distances = new NativeList<DistanceHit>(Allocator.Temp);

                    // 进行 OverlapSphere 检查，看是否已有塔占据了该位置
                    if (!physicsWorld.OverlapSphere(towerPosition, 0.1f, ref distances, CollisionFilter.Default))
                    {
                        // 如果该位置没有其他塔，则实例化新的塔
                        Entity e = ecbBOS.Instantiate(towers[placementInput.index].Prefab);
                        ecbBOS.SetComponent(e, LocalTransform.FromPosition(towerPosition));
                        //Debug.Log($"Tower successfully placed at: {towerPosition}");
                    }
                    else
                    {
                        //Debug.Log("Tower position is already occupied.");
                    }

                    // 确保在使用完 NativeList 后正确释放内存
                    distances.Dispose();
                }
            }

            // 清除输入缓冲区
            input.Clear();
        }
    }
}
