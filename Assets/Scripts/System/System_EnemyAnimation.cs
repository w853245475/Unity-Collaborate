using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct System_EnemyAnimation : ISystem
{
    private EntityManager entityManager;

    private void OnUpdate(ref SystemState systemState)
    {
        if (!SystemAPI.ManagedAPI.TryGetSingleton(out Manager_AnimationVisuals animationManager)) 
        {
            return;
        }

        entityManager = systemState.EntityManager;

        EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);

        foreach ( var (transform, componentEnemy, entity) in SystemAPI.Query<LocalTransform, Component_Enemy>().WithEntityAccess())
        {
            if(!entityManager.HasComponent<Component_VisualReference>(entity))
            {
                GameObject enemyVisual = Object.Instantiate(animationManager.enemySkeleton);

                ECB.AddComponent(entity, new Component_VisualReference { referenceObject = enemyVisual });
            }

            else
            {
                Component_VisualReference enemyVisual = entityManager.GetComponentData<Component_VisualReference>(entity);

                enemyVisual.referenceObject.transform.position = transform.Position;
                enemyVisual.referenceObject.transform.rotation = transform.Rotation;

                enemyVisual.referenceObject.GetComponent<Animator>().SetBool("IsEnemyWalking", true);
            }
        }

        ECB.Playback(entityManager);
        ECB.Dispose();

    }
}
