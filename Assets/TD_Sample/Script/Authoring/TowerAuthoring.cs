using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

/// <summary>
/// 用于存储在塔射程内的敌人实体的缓冲区
/// </summary>
public struct EnemyInRangeBuffer : IBufferElementData
{
    public Entity EnemyEntity; // 敌人实体
}

/// <summary>
/// 用于表示塔的组件数据
/// </summary>
public struct TowerComponent : IComponentData
{
    public float RotationSpeed; // 旋转速度
    public float fireRate; // 发射间隔（冷却时间）
    public float TimeSinceLastShot; // 自上次射击以来的时间（不需要手动设置）
    public Entity BulletPrefab; // 子弹预制件
    public Entity SpawnPoint; // 子弹生成点
    public float3 TargetDirection; // 当前目标方向
}

/// <summary>
/// TowerAuthoring：用于将 MonoBehaviour 数据转化为 ECS 数据的 Authoring 组件
/// </summary>
public class TowerAuthoring : MonoBehaviour
{
    public float RotationSpeed; // 设置塔的旋转速度
    public float fireRate; // 设置塔的发射速率
    public GameObject BulletPrefab; // 子弹的预制件（由设计师设置）
    public Transform SpawnPoint; // 子弹生成点（由设计师设置）

    // Baker 类用于将 UnityEngine 的数据转换为 Unity ECS 数据
    class Baker : Baker<TowerAuthoring>
    {
        public override void Bake(TowerAuthoring authoring)
        {
            // 将 MonoBehaviour 中的数据转换为 ECS Entity 数据
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // 创建 TowerComponent 并赋值
            var data = new TowerComponent
            {
                RotationSpeed = authoring.RotationSpeed,
                fireRate = authoring.fireRate,
                TimeSinceLastShot = 0f, // 初始化为 0，因为塔还没有射击
                BulletPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic),
                SpawnPoint = GetEntity(authoring.SpawnPoint, TransformUsageFlags.Dynamic),
                TargetDirection = float3.zero // 初始化为零向量
            };

            // 向 ECS 实体添加 TowerComponent 组件
            AddComponent(entity, data);

            // 向 ECS 实体添加缓冲区组件，用于存储射程内的敌人实体
            AddBuffer<EnemyInRangeBuffer>(entity);
        }
    }
}
