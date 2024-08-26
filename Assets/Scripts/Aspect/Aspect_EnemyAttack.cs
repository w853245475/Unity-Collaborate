using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements.Experimental;


namespace ROGUE.TD
{

    public readonly partial struct Aspect_EnemyAttack : IAspect
    {
        public readonly Entity entity;

        private readonly RefRW<Component_EnemyAttackProperties> _enemyAttack;
        private readonly RefRW<LocalTransform> _transform;

        private float AttackDamage => _enemyAttack.ValueRO.attackDamage;

        private float EnemyAttackTimer
        {
            get => _enemyAttack.ValueRO.attackTimer;
            set => _enemyAttack.ValueRW.attackTimer = value;
        }

        public bool IsInAttackRange(float3 playerPosition)
        {
            return math.distancesq(playerPosition, _transform.ValueRO.Position) <= _enemyAttack.ValueRO.attackRange;
        }

        public void Attack(float deltaTime, EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity playerEntity)
        {
            EnemyAttackTimer += deltaTime;

            var attackDamage = AttackDamage * deltaTime;
            var curPlayerDamage = new PlayerDamageBufferElement { Value = attackDamage };
            ecb.AppendToBuffer(sortKey, playerEntity, curPlayerDamage);
        }
    }
}

