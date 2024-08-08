
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using System;

public partial class EnemyMoveSystem : SystemBase
{
    
    // Update is called once per frame
    protected override void OnUpdate()
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        var player_entity = SystemAPI.GetSingletonEntity<Tag_PlayerBase>();


        Entities.ForEach((ref LocalTransform transform, in Component_Enemy enemy) =>
        {
            if (SystemAPI.HasComponent<LocalToWorld>(player_entity)) 
            {
                LocalToWorld targetl2w = SystemAPI.GetComponent<LocalToWorld>(player_entity);
                float3 targetPos = targetl2w.Position;

                float3 moveDirection = math.normalize(targetPos - transform.Position);
                transform.Position += moveDirection * enemy.speed * deltaTime;
            }

        }).ScheduleParallel();
    }
}
