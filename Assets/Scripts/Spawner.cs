using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Spawner : IComponentData
{
    public Entity prefab;
    public float3 spawnPosition;
    public float nextSpawnTime;
    public float spawnRate;
}