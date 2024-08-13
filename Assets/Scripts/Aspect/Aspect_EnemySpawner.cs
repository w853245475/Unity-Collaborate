using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;

namespace ROGUE.TD
{
    public readonly partial struct Aspect_EnemySpawner : IAspect
    {
        public readonly Entity entity;

        private readonly RefRW<Component_WorldMap> enemySpawnerProperties;
        //private readonly RefRW<LocalTransform> localTransform;




    }
}
