using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Authoring_Manager_AnimationVisuals : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemySkeletonPrefab;

    private class Baker_Manager_AnimationVisuals : Baker<Authoring_Manager_AnimationVisuals>
    {
        public override void Bake(Authoring_Manager_AnimationVisuals authoring)
        {
            Entity enemySkeletonPrefabEntity = GetEntity(TransformUsageFlags.None);

            AddComponentObject(enemySkeletonPrefabEntity, new Manager_AnimationVisuals
            {
                playerBasePrefab = authoring.playerPrefab,
                enemySkeleton = authoring.enemySkeletonPrefab

            });
        }
    }

}

public class Manager_AnimationVisuals : IComponentData
{

    // Add the Gameobject that needs to have animation

    public GameObject playerBasePrefab;
    public GameObject enemySkeleton;
}
