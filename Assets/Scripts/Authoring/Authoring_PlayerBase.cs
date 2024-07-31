using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class Authoring_PlayerBase : MonoBehaviour
{
    public float health;
}

public struct Component_PlayerBase : IComponentData
{
    public float health;
}

public class Baker_PlayerBase : Baker<Authoring_PlayerBase>
{
    public override void Bake(Authoring_PlayerBase authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new Component_PlayerBase
        {
            health = authoring.health
        }); ;

    }
}