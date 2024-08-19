using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public struct Component_Bullet : IComponentData
{
    public float3 Direction;
    public float Speed;
    public Entity Target;
}
public class Authoring_Bullet : MonoBehaviour
{

    public float Speed;  // 子弹的速度
    class Baker : Baker<Authoring_Bullet>
    {
        public override void Bake(Authoring_Bullet authoring)
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
