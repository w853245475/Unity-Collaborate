using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// 该结构体使用 IAspect 接口，封装了敌人的移动逻辑。
/// </summary>
public readonly partial struct EnemyMoveAspect : IAspect
{
    // 只读引用敌人的组件数据（如速度）
    public readonly RefRO<EnemyComponent> enemy;

    // 可读写引用敌人的 Transform 组件数据（如位置、旋转）
    public readonly RefRW<LocalTransform> transform;

    /// <summary>
    /// 敌人的移动逻辑，每帧根据速度和方向更新位置。
    /// </summary>
    /// <param name="dt">每帧的时间增量</param>
    public void EnemyMove(float dt)
    {
        // 获取敌人的速度
        var speed = enemy.ValueRO.Speed;

        // 定义移动方向，这里假设敌人向右移动（x 轴正方向）
        var dir = new float3(1, 0, 0);

        // 根据速度、方向和时间增量计算新的位置，并更新 Transform 组件
        transform.ValueRW = transform.ValueRO.Translate(speed * dir * dt);
    }
}
