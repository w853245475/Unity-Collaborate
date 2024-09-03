using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Component_NavAgent : IComponentData
{
    public bool pathCalculated;
    public int currentWaypoint;
    public float nextPathCalculateTime;
}

public struct Buffer_Waypoint : IBufferElementData
{
    public float3 wayPoint;
}
