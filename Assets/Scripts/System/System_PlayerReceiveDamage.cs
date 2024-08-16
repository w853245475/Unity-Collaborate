using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Physics;
using Unity.Collections;
using static ROGUE.TD.System_EnemySpawner;
using Unity.Physics.Stateful;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Assertions;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(StatefulTriggerEventBufferSystem))]
public partial class TriggerVolumeChangeMaterialSystem : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem m_CommandBufferSystem;
    private EntityQuery m_NonTriggerQuery;
    private EntityQueryMask m_NonTriggerMask;

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

    protected override void OnUpdate()
    {
        EntityCommandBuffer commandBuffer = m_CommandBufferSystem.CreateCommandBuffer();

        // Need this extra variable here so that it can
        // be captured by Entities.ForEach loop below
        var nonTriggerMask = m_NonTriggerMask;


        foreach (var (triggerEventBuffer, changeMaterial, entity) in SystemAPI.Query<DynamicBuffer<StatefulTriggerEvent>, RefRW<Component_PlayerBase>>().WithEntityAccess())
        {
            for (int i = 0; i < triggerEventBuffer.Length; i++)
            {
                var triggerEvent = triggerEventBuffer[i];
                var otherEntity = triggerEvent.GetOtherEntity(entity);

                // exclude other triggers and processed events
                if (triggerEvent.State == StatefulEventState.Stay || !nonTriggerMask.MatchesIgnoreFilter(otherEntity))
                {
                    //if (SystemAPI.GetComponent<Component_Enemy>(otherEntity))
                    //{

                    //    Debug.Log(otherEntity);

                    //    continue;
                    //}

                }

                if (triggerEvent.State == StatefulEventState.Enter)
                {
                    Debug.Log("Enter");
                    //MaterialMeshInfo volumeMaterialInfo = materialMeshInfoFromEntity[entity];
                    //RenderMeshArray volumeRenderMeshArray = EntityManager.GetSharedComponentManaged<RenderMeshArray>(entity);

                    //MaterialMeshInfo otherMaterialMeshInfo = materialMeshInfoFromEntity[otherEntity];

                    //otherMaterialMeshInfo.Material = volumeMaterialInfo.Material;

                    //commandBuffer.SetComponent(otherEntity, otherMaterialMeshInfo);
                }
                else
                {
                    //// State == PhysicsEventState.Exit
                    //if (changeMaterial.ValueRW.ReferenceEntity == Entity.Null)
                    //{
                    //    continue;
                    //}

                    //MaterialMeshInfo otherMaterialMeshInfo = materialMeshInfoFromEntity[otherEntity];
                    //MaterialMeshInfo referenceMaterialMeshInfo = materialMeshInfoFromEntity[changeMaterial.ValueRW.ReferenceEntity];
                    //RenderMeshArray referenceRenderMeshArray = EntityManager.GetSharedComponentManaged<RenderMeshArray>(changeMaterial.ValueRW.ReferenceEntity);

                    //otherMaterialMeshInfo.Material = referenceMaterialMeshInfo.Material;

                    //commandBuffer.SetComponent(otherEntity, otherMaterialMeshInfo);
                }
            }
        }

        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}