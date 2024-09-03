using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Component_Bullet : IComponentData
{
    public float3 Direction;        // 子弹的飞行方向
    public float Speed;             // 子弹的速度
    public Entity Target;           // 目标实体
    public DebuffType DebuffType;   // 子弹携带的Debuff类型
    public float DebuffDuration;    // Debuff的持续时间
    public float DebuffIntensity;   // Debuff的强度
}

public class Authoring_Bullet : MonoBehaviour
{
    public float Speed;             // 子弹的速度
    public DebuffType DebuffType;   // 子弹携带的Debuff类型（由Inspector中设置）
    public float DebuffDuration;    // Debuff持续时间
    public float DebuffIntensity;   // Debuff强度

    class Authoring_BulletBaker : Baker<Authoring_Bullet>
    {
        public override void Bake(Authoring_Bullet authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var data = new Component_Bullet
            {
                Speed = authoring.Speed,
                Direction = float3.zero,
                Target = Entity.Null,
                DebuffType = authoring.DebuffType,
                DebuffDuration = authoring.DebuffDuration,
                DebuffIntensity = authoring.DebuffIntensity
            };
            //Debug.Log($"Baking Bullet: Speed={data.Speed}, DebuffType={data.DebuffType}, Duration={data.DebuffDuration}, Intensity={data.DebuffIntensity}");
            AddComponent(entity, data);

        }
    }
}
