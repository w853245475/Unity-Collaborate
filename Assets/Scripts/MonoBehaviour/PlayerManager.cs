using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public InputAction Input;
    public Camera Camera;
    public int TowerIndex;
    public PhysicsCategoryTags BelongsTo;
    public PhysicsCategoryTags CollidesWith;

    private Entity Entity;
    private World World;

    private void OnEnable()
    {
        if (Input == null)
        {
            Debug.LogError("Input action is not assigned.");
            return;
        }

        Input.started += MouseClicked;
        Input.Enable();

        Camera = Camera == null ? Camera.main : Camera;
        if (Camera == null)
        {
            Debug.LogError("Main Camera is not assigned or found.");
        }

        World = World.DefaultGameObjectInjectionWorld;
        if (World == null)
        {
            Debug.LogError("Default World is not created or found.");
        }

    }
    private void MouseClicked(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = context.ReadValue<Vector2>();
        UnityEngine.Ray ray = Camera.ScreenPointToRay(screenPosition);
        Debug.Log(ray.GetPoint(Camera.farClipPlane));

        if (World.IsCreated && !World.EntityManager.Exists(Entity))
        {
            Entity = World.EntityManager.CreateEntity();
            World.EntityManager.AddBuffer<TowerPlacementInput>(Entity);
        }
        CollisionFilter filter = CollisionFilter.Default;
        filter.BelongsTo = BelongsTo.Value;
        filter.CollidesWith = CollidesWith.Value;

        RaycastInput input = new RaycastInput() {

            Start = ray.origin,
            Filter = filter,
            End = ray.GetPoint(Camera.farClipPlane)


        };

        World.EntityManager.GetBuffer<TowerPlacementInput>(Entity).Add(new TowerPlacementInput() { Value = input,index = TowerIndex });

    }

    private void OnDisable()
    {
        Input.started -= MouseClicked;
        Input.Disable();
        if (World.IsCreated && World.EntityManager.Exists(Entity))
        {
            World.EntityManager.DestroyEntity(Entity);
        }
    }
}
 public struct TowerPlacementInput:IBufferElementData
{

    public RaycastInput Value;
    internal int index;
}
