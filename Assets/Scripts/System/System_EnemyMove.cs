
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using System;
using Unity.Burst;
using ROGUE.TD;

public partial class EnemyMoveSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<Tag_PlayerBase>();
        RequireForUpdate<Component_Enemy>();
    }

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

                if(math.distance(targetPos, transform.Position) > 0.5)
                {
                    float3 moveDirection = math.normalize(targetPos - transform.Position);
                    transform.Position += moveDirection * enemy.speed * deltaTime;
                }


            }

        }).ScheduleParallel();
    }


    //[BurstCompile]
    //public partial struct Job_EnemyWalk : IJobEntity
    //{
    //    public float DeltaTime;
    //    public float BrainRadiusSq;
    //    public EntityCommandBuffer.ParallelWriter ECB;

    //    [BurstCompile]
    //    private void Execute(Aspect_EnemyWalk zombie, [EntityIndexInQuery] int sortKey)
    //    {
    //        zombie.Walk(DeltaTime);
    //        if (zombie.IsInStoppingRange(float3.zero, BrainRadiusSq))
    //        {
    //            ECB.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, false);
    //            ECB.SetComponentEnabled<ZombieEatProperties>(sortKey, zombie.Entity, true);
    //        }
    //    }
    //}

}
