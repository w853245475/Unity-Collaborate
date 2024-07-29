using UnityEngine;
using System.Collections;
using System.Diagnostics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class MovingSystemBase : SystemBase
{


    // Update is called once per frame
    protected override void OnUpdate()
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        Entities.ForEach((ref LocalTransform transform, in Speed speed) =>
        {
            transform.Position += new float3(1, 0, 0) * speed.value * deltaTime;
        }).ScheduleParallel();
    }
}
