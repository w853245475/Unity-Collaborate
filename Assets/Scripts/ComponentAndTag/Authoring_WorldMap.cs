using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;





public struct Component_WorldMap : IComponentData
{

}


public class Authoring_WorldMap : MonoBehaviour
{

}

public class Baker_WorldMap : Baker<Authoring_WorldMap>
{
    public override void Bake(Authoring_WorldMap authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new Component_WorldMap
        {

        });
        //AddComponent(new GraveyardRandom
        //{
        //    Value = Random.CreateFromIndex(authoring.RandomSeed)
        //});
        //AddComponent<ZombieSpawnPoints>();
        //AddComponent<ZombieSpawnTimer>();
    }
}
