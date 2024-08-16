using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

/// <summary>
/// 系统负责更新所有具有 EnemyComponent 的实体的移动逻辑。
/// </summary>
[BurstCompile]
public partial struct EnemyMovementSystem : ISystem
{
    /// <summary>
    /// 每帧调用 OnUpdate 来更新敌人的位置。
    /// </summary>
    public void OnUpdate(ref SystemState state)
    {
        // 获取每帧的时间增量，用于平滑移动
        var dt = SystemAPI.Time.DeltaTime;

        // 遍历所有具有 EnemyComponent 的实体，并通过 EnemyMoveAspect 进行移动更新
        foreach (EnemyMoveAspect enemyMoveAspect in
                     SystemAPI.Query<EnemyMoveAspect>()
                         .WithAll<EnemyComponent>())
        {
            // 调用敌人移动方法
            enemyMoveAspect.EnemyMove(dt);
        }
    }
}
