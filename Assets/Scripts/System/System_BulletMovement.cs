using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct System_BulletMovement : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Component_Bullet>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (bullet, transform) in SystemAPI.Query<RefRW<Component_Bullet>, RefRW<LocalTransform>>())
        {
         
            transform.ValueRW.Position += new float3(bullet.ValueRW.Direction.x, bullet.ValueRW.Direction.y, bullet.ValueRW.Direction.z) * bullet.ValueRW.Speed * deltaTime;

            // 如果需要旋转子弹，可以基于X和Z轴的方向计算角度并旋转子弹
            // 这里移除了Y轴的影响
            //float angle = math.atan2(bullet.ValueRW.Direction.x, bullet.ValueRW.Direction.z) * (180f / math.PI);
            //transform.ValueRW.Rotation = quaternion.Euler(0f, math.radians(angle), 0f); // 在Y轴上旋转
        }
    }
}


