using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 表示敌人的组件数据，包括健康值和移动速度
/// </summary>
public struct EnemyComponent : IComponentData
{
    public float Health; // 敌人的当前生命值
    public float Speed;  // 敌人的移动速度
}

/// <summary>
/// 用于将 UnityEngine 中的敌人对象转换为 ECS 实体的 Authoring 组件
/// </summary>
public class EnemyAuthoring : MonoBehaviour
{
    // 在 Unity 编辑器中设置敌人的速度和健康值
    public float Speed;
    public float Health;

    /// <summary>
    /// Baker 类将 MonoBehaviour 中的数据转化为 ECS 的组件数据
    /// </summary>
    class Baker : Baker<EnemyAuthoring>
    {
        // Bake 方法用于将 MonoBehaviour 中的数据转换为 ECS 实体数据
        public override void Bake(EnemyAuthoring authoring)
        {
            // 创建或获取对应的 ECS 实体
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // 创建 EnemyComponent 并从 MonoBehaviour 中获取初始值
            var data = new EnemyComponent
            {
                Speed = authoring.Speed,
                Health = authoring.Health
            };

            // 将 EnemyComponent 添加到该实体中
            AddComponent(entity, data);
        }
    }
}
