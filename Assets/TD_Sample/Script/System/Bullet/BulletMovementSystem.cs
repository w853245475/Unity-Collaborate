using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct BulletMovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BulletComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (bullet, transform) in SystemAPI.Query<RefRW<BulletComponent>, RefRW<LocalTransform>>())
        {
            // 直接更新子弹的位置，沿着方向按速度移动
            transform.ValueRW.Position += bullet.ValueRW.Direction * bullet.ValueRW.Speed * deltaTime;
        }
    }
}
