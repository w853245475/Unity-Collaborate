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
            foreach (var brainDamageBufferElement in _playerDamageBuffer)
            {
                _player.ValueRW.health -= brainDamageBufferElement.Value;
            }
            _playerDamageBuffer.Clear();

            //Debug.Log(_player.ValueRW.health);
        }

    }
}
