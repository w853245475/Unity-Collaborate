using ROGUE.TD;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.Rendering;

[BurstCompile]
public partial struct System_NavAgent : ISystem
{
    [BurstCompile]
    private void OnUpdate(ref SystemState state)
    {
        var player_entity = SystemAPI.GetSingletonEntity<Tag_PlayerBase>();
        float3 targetPosition = state.EntityManager.GetComponentData<LocalTransform>(player_entity).Position;

        foreach (var (navAgent, transform, enemy, enemyAttackAspect, entity) in
            SystemAPI.Query<RefRW<Component_NavAgent>, RefRW<LocalTransform>, RefRW<Component_Enemy>, Aspect_EnemyAttack>().WithEntityAccess())
        {
            if (!enemyAttackAspect.IsInAttackRange(targetPosition))
            {
                DynamicBuffer<Buffer_Waypoint> wayPointBuffer = state.EntityManager.GetBuffer<Buffer_Waypoint>(entity);

                if (navAgent.ValueRO.nextPathCalculateTime < SystemAPI.Time.ElapsedTime)
                {
                    navAgent.ValueRW.nextPathCalculateTime += 1;
                    navAgent.ValueRW.pathCalculated = false;
                    CalculatePath(navAgent, transform, wayPointBuffer, ref state, targetPosition);
                }
                else
                {
                    Move(navAgent, transform, wayPointBuffer, ref state, enemy);
                }
            }
        }    
    }

    [BurstCompile]
    private void Move(RefRW<Component_NavAgent> navAgent, RefRW<LocalTransform> transform, DynamicBuffer<Buffer_Waypoint> waypointBuffer,
    ref SystemState state, RefRW<Component_Enemy> enemy)
    {
        if (math.distance(transform.ValueRO.Position, waypointBuffer[navAgent.ValueRO.currentWaypoint].wayPoint) < 0.4f)
        {
            if (navAgent.ValueRO.currentWaypoint + 1 < waypointBuffer.Length)
            {
                navAgent.ValueRW.currentWaypoint += 1;
            }
        }

        float3 direciton = waypointBuffer[navAgent.ValueRO.currentWaypoint].wayPoint - transform.ValueRO.Position;

        //float angle = math.degrees(math.atan2(direciton.z, direciton.x));

        //transform.ValueRW.Rotation = math.slerp(
        //                transform.ValueRW.Rotation,
        //                quaternion.Euler(new float3(0, angle, 0)),
        //                SystemAPI.Time.DeltaTime);

        transform.ValueRW.Position += math.normalize(direciton) * SystemAPI.Time.DeltaTime * enemy.ValueRO.speed;
    }

    [BurstCompile]
    private void CalculatePath(RefRW<Component_NavAgent> navAgent, RefRW<LocalTransform> transform, 
        DynamicBuffer<Buffer_Waypoint> wayPointBuffer, ref SystemState state, float3 targetPosition)
    {
        NavMeshQuery query = new NavMeshQuery(NavMeshWorld.GetDefaultWorld(), Allocator.TempJob, 1000);

        float3 fromPosition = transform.ValueRO.Position;
        float3 extends = new float3(1, 1, 1);

        NavMeshLocation fromLocation = query.MapLocation(fromPosition, extends, 0);
        NavMeshLocation toLocation = query.MapLocation(targetPosition, extends, 0);

        PathQueryStatus status;
        PathQueryStatus returningStatus;
        int maxPathSize = 100;


        if(query.IsValid(fromLocation) && query.IsValid(toLocation))
        {
            status = query.BeginFindPath(fromLocation, toLocation);

            if(status == PathQueryStatus.InProgress)
            {
                status = query.UpdateFindPath(100, out int iterationPerformed);

                if (status == PathQueryStatus.Success)
                {
                    status = query.EndFindPath(out int pathSize);

                    NativeArray<NavMeshLocation> result = new NativeArray<NavMeshLocation>(pathSize + 1, Allocator.Temp);
                    NativeArray<StraightPathFlags> straightPathFlags = new NativeArray<StraightPathFlags>(maxPathSize, Allocator.Temp);
                    NativeArray<float> vertexSide = new NativeArray<float>(maxPathSize, Allocator.Temp);
                    NativeArray<PolygonId> polygonIds = new NativeArray<PolygonId>(pathSize + 1, Allocator.Temp);
                    int straightPathCount = 0;

                    query.GetPathResult(polygonIds);

                    returningStatus = PathUtils.FindStraightPath
                        (
                        query, fromPosition, targetPosition, polygonIds, pathSize,
                        ref result, ref straightPathFlags, ref vertexSide, ref straightPathCount, maxPathSize
                        );

                    if (returningStatus == PathQueryStatus.Success)
                    {
                        wayPointBuffer.Clear();

                        foreach (NavMeshLocation location in result)
                        {
                            if (location.position != Vector3.zero)
                            {
                                wayPointBuffer.Add(new Buffer_Waypoint { wayPoint = location.position });
                            }
                        }

                        navAgent.ValueRW.currentWaypoint = 0;
                        navAgent.ValueRW.pathCalculated = true;
                    }

                    straightPathFlags.Dispose();
                    polygonIds.Dispose();
                    vertexSide.Dispose();
                }

            }


        }
        query.Dispose();
    }

}
