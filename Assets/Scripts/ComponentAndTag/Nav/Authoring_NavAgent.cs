using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Authoring_NavAgent : MonoBehaviour
{
    [SerializeField]
    private Transform targetTrasform;
    [SerializeField]
    private float moveSpeed;

    private class Baker_NavAgent : Baker<Authoring_NavAgent>
    {
        public override void Bake(Authoring_NavAgent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Component_NavAgent
            {
                targetEntity = GetEntity(authoring.targetTrasform, TransformUsageFlags.Dynamic),
                moveSpeed = authoring.moveSpeed
            });
            AddBuffer<Buffer_Waypoint>(entity);
        }
    }
}
