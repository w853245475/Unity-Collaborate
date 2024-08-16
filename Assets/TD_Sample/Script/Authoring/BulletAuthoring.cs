using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// BulletComponent 用于表示子弹在 ECS 中的数据组件，包含子弹的移动方向、速度和目标实体。
/// </summary>
public struct BulletComponent : IComponentData
{
    public float3 Direction;  // 子弹移动的方向
    public float Speed;       // 子弹的速度
    public Entity Target;     // 子弹的目标实体，可以为空（Entity.Null）
}

/// <summary>
/// BulletAuthoring 是 MonoBehaviour 脚本，用于在编辑器中设置子弹的属性。
/// 通过 Baker 类将 Unity 的 MonoBehaviour 转换为 ECS 的 BulletComponent。
/// </summary>
public class BulletAuthoring : MonoBehaviour
{
    [Tooltip("设置子弹的速度")]
    public float Speed;  // 子弹的速度，通过 Unity 编辑器设置

    /// <summary>
    /// BulletBaker 将 BulletAuthoring 中的属性转换为 ECS 的 BulletComponent。
    /// 这是 ECS 与传统 Unity 引擎之间的桥梁。
    /// </summary>
    class BulletBaker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            // 获取当前实体（Entity），使用动态转换标志
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // 将 BulletAuthoring 中的数据转移到 ECS 的 BulletComponent
            var data = new BulletComponent
            {
                Speed = authoring.Speed,  // 从 Unity Inspector 中读取的速度
                Direction = float3.zero,  // 初始化方向为零向量，后续可在子弹逻辑中赋值
                Target = Entity.Null      // 初始目标为空，可以在子弹逻辑中设置具体目标
            };

            // 将 BulletComponent 添加到当前实体中
            AddComponent(entity, data);
        }
    }
}
