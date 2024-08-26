using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ROGUE.TD
{
    [InternalBufferCapacity(8)]
    public struct PlayerDamageBufferElement : IBufferElementData
    {
        public float Value;
    }
}