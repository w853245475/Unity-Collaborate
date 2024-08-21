using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


namespace ROGUE.TD
{

    public readonly partial struct Aspect_EnemyAttack : IAspect
    {
        public readonly Entity entity;

        private readonly RefRW<Component_EnemyAttackProperties> _enemyAttack;



        private float EnemyAttackTimer
        {
            get => _enemyAttack.ValueRO.attackTimer;
            set => _enemyAttack.ValueRW.attackTimer = value;
        }
    }
}

