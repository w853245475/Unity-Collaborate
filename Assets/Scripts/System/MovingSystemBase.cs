
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using System;

public partial class MovingSystemBase : SystemBase
{
    
    // Update is called once per frame
    protected override void OnUpdate()
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        Entities.ForEach((ref LocalTransform transform, in Component_Enemy enemy) =>
        {
            if (SystemAPI.HasComponent<LocalToWorld>(enemy.target)) 
            {

                LocalToWorld targetl2w = SystemAPI.GetComponent<LocalToWorld>(enemy.target);
                float3 targetPos = targetl2w.Position;

                float3 moveDirection = math.normalize(targetPos - transform.Position);
                transform.Position += moveDirection * enemy.speed * deltaTime;
            }

        }).ScheduleParallel();
    }
}
