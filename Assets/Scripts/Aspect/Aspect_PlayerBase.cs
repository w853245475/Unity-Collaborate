using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


namespace ROGUE.TD
{
    public readonly partial struct Aspect_PlayerBase : IAspect
    {
        public readonly Entity entity;

        private readonly RefRW<Component_PlayerBase> _player;

        private readonly DynamicBuffer<PlayerDamageBufferElement> _playerDamageBuffer;


        public void PlayerReceiveDamage()
        {
            foreach (var playerDamageBufferElement in _playerDamageBuffer)
            {
                _player.ValueRW.health -= playerDamageBufferElement.Value;
            }
            _playerDamageBuffer.Clear();

            if (_player.ValueRW.health <= 0)
            {

            }

            //Debug.Log(_player.ValueRW.health);
        }

        public void DestroyPlayerEntity()
        {

        }

    }
}
