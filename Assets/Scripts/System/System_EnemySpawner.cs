using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct System_EnemySpawner : ISystem
{
    public void OnCreate(ref SystemState state) { }
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) 
    { 
        foreach(RefRW<Spawner_Enemy> spawner in SystemAPI.Query<RefRW<Spawner_Enemy>>())
        {
            ProcessSpawner(ref state, spawner);
        }

    }

    private void ProcessSpawner(ref SystemState state, RefRW<Spawner_Enemy> spawner)
    {
        if(spawner.ValueRO.nextSpawnTime < SystemAPI.Time.ElapsedTime)
        {
            Entity instantiated = state.EntityManager.Instantiate(spawner.ValueRO.prefab);
            state.EntityManager.SetComponentData(instantiated, LocalTransform.FromPosition(spawner.ValueRO.spawnPosition));
            //state.EntityManager.SetComponentData(instantiated, LocalTransform.FromScale(5));
            spawner.ValueRW.nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + spawner.ValueRO.spawnRate;
        }
    }
}