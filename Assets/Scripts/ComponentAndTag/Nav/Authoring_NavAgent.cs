using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Authoring_NavAgent : MonoBehaviour
{
    private class Baker_NavAgent : Baker<Authoring_NavAgent>
    {
        public override void Bake(Authoring_NavAgent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Component_NavAgent { });
            AddBuffer<Buffer_Waypoint>(entity);
        }
    }
}
