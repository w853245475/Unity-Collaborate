using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Tower Component
/// </summary>
public struct Component_Tower : IComponentData
{
    //public float RotationSpeed; // The speed at which the tower rotates
    public float FireRate; // The interval between shots (cooldown period)
    public float TimeSinceLastShot; // The time elapsed since the last shot (automatically managed)
    public float3 TargetDirection;
}

public struct Component_BulletData : IComponentData
{
    public Entity BulletPrefab;
    public Entity SpawnPoint;
}
public struct Component_EnemyDetection : IBufferElementData
{
    public Entity EnemyEntity;
}
public struct Component_AttackEffect : IComponentData
{
    public DebuffType EffectType;
    //public float EffectValue;  
    public float Duration;      
}
public struct Component_ElementalAttribute : IComponentData
{
    public ElementType ElementType;
    //public float Strength;
}

public class Authoring_Tower : MonoBehaviour
{
    // Fields to expose in the Unity Inspector
    //public float RotationSpeed = 1.0f;
    public float FireRate = 1.0f;
    public GameObject BulletPrefab;
    public Transform SpawnPoint;
    public ElementType ElementType;
    public DebuffType AttackEffectType;
    public float EffectDuration;

    class Baker : Baker<Authoring_Tower>
    {
        public override void Bake(Authoring_Tower authoring)
        {
            // Get the Entity
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Add the Tower component
            var towerComponent = new Component_Tower
            {
                //RotationSpeed = authoring.RotationSpeed,
                FireRate = authoring.FireRate,
                TimeSinceLastShot = 0f, // Initial value should be 0
                TargetDirection = float3.zero // 初始化为零向量
            };
            AddComponent(entity, towerComponent);

            // Add BulletData component
            var bulletDataComponent = new Component_BulletData
            {
                BulletPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic),
                SpawnPoint = GetEntity(authoring.SpawnPoint, TransformUsageFlags.Dynamic)
            };
            AddComponent(entity, bulletDataComponent);

            // Add ElementalAttribute component
            var elementalComponent = new Component_ElementalAttribute
            {
                ElementType = authoring.ElementType
            };
            AddComponent(entity, elementalComponent);

            // Add AttackEffect component
            var attackEffectComponent = new Component_AttackEffect
            {
                EffectType = authoring.AttackEffectType,
                Duration = authoring.EffectDuration
            };
            AddComponent(entity, attackEffectComponent);

            // Add an empty buffer for Enemy Detection
            AddBuffer<Component_EnemyDetection>(entity);
        }
    }
}
