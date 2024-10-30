using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ROGUE.TD;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

[TestFixture]
public class Test_EnemyAttack : CustomECSTestFixture
{

    private World m_TestWorld;
    private EntityManager m_EntityManager;
    private System_EnemyAttack m_EnemyAttackSystem;
    private EndSimulationEntityCommandBufferSystem m_EndSimulationECBSystem;

    [SetUp]
    public override void Setup()
    {
        base.Setup();

        m_TestWorld = new World("TestWorld");
        m_EntityManager = m_TestWorld.EntityManager;
        m_EndSimulationECBSystem = m_TestWorld.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        CreateSystem<System_EnemyAttack>();


    }

    // A Test behaves as an ordinary method
    [Test]
    public void Test_EnemyAttackSimplePasses()
    {
        var systema = World.GetOrCreateSystem<System_EnemyAttack>();

        var em = EmptySystem.EntityManager;

        World.Update();

        // Create player entity
        var playerEntity = CreateEntity(typeof(Component_PlayerBase), typeof(LocalTransform), typeof(PlayerDamageBufferElement));
        Manager.AddComponentData(playerEntity, new LocalTransform { Position = new float3(0, 0, 0) });

        // Create enemy entity with necessary components
        var enemyEntity = CreateEntity(typeof(Component_EnemyAttackProperties), typeof(LocalTransform));
        Manager.AddComponentData(enemyEntity, new LocalTransform { Position = new float3(1, 0, 0) });

        // Set up enemy attack properties
        Manager.AddComponentData(enemyEntity, new Component_EnemyAttackProperties
        {
            attackDamage = 10f,
            attackRange = 2f,
            attackTimer = 0f
        });


        // UpdateSystem<System_EnemyAttack>();

        // // Assert that the player received damage
        // Assert.IsTrue(m_EntityManager.HasComponent<PlayerDamageBufferElement>(playerEntity));
        // var damageBuffer = m_EntityManager.GetBuffer<PlayerDamageBufferElement>(playerEntity);
        // Assert.IsTrue(damageBuffer.Length > 0);
        // Assert.AreEqual(10f, damageBuffer[0].Value);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator Test_EnemyAttackWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
