using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;

[BurstCompile]
public partial struct System_ApplyDebuff : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Component_Enemy>();
        state.RequireForUpdate<Component_EnemyDebuff>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        foreach (var (enemy, entity) in SystemAPI.Query<RefRW<Component_Enemy>>().WithEntityAccess())
        {
            var debuffBuffer = state.EntityManager.GetBuffer<Component_EnemyDebuff>(entity);

            for (int i = debuffBuffer.Length - 1; i >= 0; i--)
            {
                var debuff = debuffBuffer[i];
                ApplyDebuff(ref enemy.ValueRW, debuff, ref debuffBuffer, i);

                debuff.Duration -= SystemAPI.Time.DeltaTime;
                if (debuff.Duration <= 0)
                {
                    debuffBuffer.RemoveAt(i);
                }
                else
                {
                    debuffBuffer[i] = debuff;
                }
            }

            ecb.SetComponent(0, entity, enemy.ValueRW);  // 直接使用0作为jobIndex
        }
    }

    private void ApplyDebuff(ref Component_Enemy enemy, Component_EnemyDebuff debuff, ref DynamicBuffer<Component_EnemyDebuff> debuffBuffer, int debuffIndex)
    {
        switch (debuff.Type)
        {
            case DebuffType.Slow:
                // 应用减速效果
                enemy.speed *= (1.0f - debuff.Intensity);
                //Debug.Log($"Applying Slow debuff. New speed: {enemy.speed}");
                break;
            case DebuffType.Burn:
                enemy.health *= (1.0f + debuff.Intensity);
                break;
        }
    }
}
