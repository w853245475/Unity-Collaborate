using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ROGUE.TD;
using Unity.Entities;

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
        // Use the Assert class to test conditions
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
