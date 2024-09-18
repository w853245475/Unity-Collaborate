using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Physics.Stateful;
using Unity.Physics.Systems;
using Unity.Assertions;
using ROGUE.TD;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(StatefulTriggerEventBufferSystem))]
public partial class TriggerVolumeChangeMaterialSystem : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem m_CommandBufferSystem;
    private EntityQuery m_NonTriggerQuery;
    private EntityQueryMask m_NonTriggerMask;

    [BurstCompile]
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetOrCreateSystemManaged<EndFixedStepSimulationEntityCommandBufferSystem>();

        m_NonTriggerQuery =
            GetEntityQuery(new EntityQueryDesc
            {
                None = new ComponentType[]
                {
                    typeof(StatefulTriggerEvent)
                }
            });
        Assert.IsFalse(m_NonTriggerQuery.HasFilter(), "The use of EntityQueryMask in this system will not respect the query's active filter settings.");
        m_NonTriggerMask = m_NonTriggerQuery.GetEntityQueryMask();

        RequireForUpdate<Component_PlayerBase>();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        EntityCommandBuffer commandBuffer = m_CommandBufferSystem.CreateCommandBuffer();

        // Need this extra variable here so that it can
        // be captured by Entities.ForEach loop below
        var nonTriggerMask = m_NonTriggerMask;

        foreach (var player in SystemAPI.Query<Aspect_PlayerBase>())
        {
            player.PlayerReceiveDamage();
        }


        //foreach (var (triggerEventBuffer, changeMaterial, entity) in SystemAPI.Query<DynamicBuffer<StatefulTriggerEvent>, RefRW<Component_PlayerBase>>().WithEntityAccess())
        //{
        //    for (int i = 0; i < triggerEventBuffer.Length; i++)
        //    {
        //        var triggerEvent = triggerEventBuffer[i];
        //        var otherEntity = triggerEvent.GetOtherEntity(entity);


        //        if (EntityManager.HasComponent<Component_Enemy>(otherEntity))
        //        {
        //            var deltaTime = SystemAPI.Time.DeltaTime;
        //            var enemyComponent = SystemAPI.GetComponent<Component_Enemy>(otherEntity);

        //            // exclude other triggers and processed events
        //            if (triggerEvent.State == StatefulEventState.Stay || !nonTriggerMask.MatchesIgnoreFilter(otherEntity))
        //            {

        //                //if (enemyComponent.attackTimer > 0)
        //                //{
        //                //    Debug.Log($"{enemyComponent.attackTimer}");
        //                //    enemyComponent.attackTimer -= deltaTime;
        //                //    return;
        //                //}
        //                //else
        //                //{
        //                //    enemyComponent.attackTimer = enemyComponent.attackRate;
        //                //    Debug.Log(enemyComponent.attackTimer);
        //                //}

        //                //Job.WithCode(() =>
        //                //    {

        //                //        Debug.Log(enemyComponent.attackTimer);
        //                //        if (enemyComponent.attackTimer > 0)
        //                //        {
        //                //            Debug.Log($"{enemyComponent.attackTimer}");
        //                //            enemyComponent.attackTimer -= deltaTime;
        //                //            return;
        //                //        }
        //                //        else
        //                //        {
        //                //            enemyComponent.attackTimer = enemyComponent.attackRate;
        //                //            Debug.Log(enemyComponent.attackTimer);
        //                //        }



        //                //    }
        //                //).Run();


        //                continue;

        //            }

        //            if (triggerEvent.State == StatefulEventState.Enter)
        //            {
        //                //Debug.Log("Enter");
        //                //MaterialMeshInfo volumeMaterialInfo = materialMeshInfoFromEntity[entity];
        //                //RenderMeshArray volumeRenderMeshArray = EntityManager.GetSharedComponentManaged<RenderMeshArray>(entity);

        //                //MaterialMeshInfo otherMaterialMeshInfo = materialMeshInfoFromEntity[otherEntity];

        //                //otherMaterialMeshInfo.Material = volumeMaterialInfo.Material;

        //                //commandBuffer.SetComponent(otherEntity, otherMaterialMeshInfo);
        //            }
        //            else
        //            {
        //                //// State == PhysicsEventState.Exit
        //                //if (changeMaterial.ValueRW.ReferenceEntity == Entity.Null)
        //                //{
        //                //    continue;
        //                //}

        //                //MaterialMeshInfo otherMaterialMeshInfo = materialMeshInfoFromEntity[otherEntity];
        //                //MaterialMeshInfo referenceMaterialMeshInfo = materialMeshInfoFromEntity[changeMaterial.ValueRW.ReferenceEntity];
        //                //RenderMeshArray referenceRenderMeshArray = EntityManager.GetSharedComponentManaged<RenderMeshArray>(changeMaterial.ValueRW.ReferenceEntity);

        //                //otherMaterialMeshInfo.Material = referenceMaterialMeshInfo.Material;

        //                //commandBuffer.SetComponent(otherEntity, otherMaterialMeshInfo);
        //            }
        //        }

        //    }
        //}

        //m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }


    [BurstCompile]
    public partial struct Job_PlayerReceiveDamage : IJobEntity
    {
        public float deltaTime;
        public EntityCommandBuffer ECB;
        public Component_Enemy enemyComponent;

        [BurstDiscard]
        private void DebugInfo()
        {
            Debug.Log("<b> <size=13> <color=#9DF155>Info : 3 SetDataSystem : Setting Data .</color> </size> </b>");
        }

        [BurstCompile]
        private void Execute(Aspect_EnemySpawner enemySpawner)
        {
            enemySpawner.EnemySpawnTimer -= deltaTime;


            if (enemySpawner.EnemySpawnTimer > 0) return;

            DebugInfo();

            var newZombie = ECB.Instantiate(enemySpawner.SkeletonEnemyPrefab);
            ECB.SetComponent(newZombie, LocalTransform.FromPositionRotationScale(enemySpawner.spawnPosition, Quaternion.identity, 5));
            enemySpawner.EnemySpawnTimer = enemySpawner.EnemySpawnRate;
        }
    }
}