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
        // 获取屏幕点击位置
        Vector2 screenPosition = context.ReadValue<Vector2>();
        //Debug.Log($"Screen Position: {screenPosition}");

        // 将屏幕位置转换为射线
        UnityEngine.Ray ray = Camera.ScreenPointToRay(screenPosition);
        //Debug.Log($"Ray Origin: {ray.origin}, Ray Direction: {ray.direction}");

        // 使用 UnityEngine.Plane，检查地面平面是否被击中
        UnityEngine.Plane groundPlane = new UnityEngine.Plane(Vector3.up, 0); // 假设Y=0为地面
        float enter = 0.0f;

        if (groundPlane.Raycast(ray, out enter))
        {
            // 获取地面上的点击点
            Vector3 hitPoint = ray.GetPoint(enter);
           // Debug.Log($"Hit Point on Ground: {hitPoint}");

            // 在 Scene 中绘制一条从射线起点到地面击中点的可视化线条
            //Debug.DrawLine(ray.origin, hitPoint, Color.red, 10.0f); // 红色线条，持续2秒

            // 确保World和Entity的状态
            if (World.IsCreated && !World.EntityManager.Exists(Entity))
            {
                //Debug.Log("World is created, and Entity does not exist. Creating Entity...");
                // 创建实体并添加缓冲区
                Entity = World.EntityManager.CreateEntity();
                World.EntityManager.AddBuffer<TowerPlacementInput>(Entity);
                //Debug.Log("Entity created and TowerPlacementInput buffer added.");
            }
            else
            {
                //if (!World.IsCreated)
                   // Debug.LogError("World is not created.");
                //if (World.EntityManager.Exists(Entity))
                //    //Debug.Log("Entity already exists.");
            }

            // 设置碰撞过滤器
            CollisionFilter filter = CollisionFilter.Default;
            filter.BelongsTo = BelongsTo.Value;
            filter.CollidesWith = CollidesWith.Value;
            //Debug.Log($"Collision Filter set: BelongsTo={filter.BelongsTo}, CollidesWith={filter.CollidesWith}");

            // 创建RaycastInput
            RaycastInput input = new RaycastInput()
            {
                Start = ray.origin,
                End = hitPoint, // 使用击中地面的位置
                Filter = filter
            };
            //Debug.Log($"RaycastInput created: Start={input.Start}, End={input.End}");

            // 将RaycastInput添加到缓冲区
            World.EntityManager.GetBuffer<TowerPlacementInput>(Entity).Add(new TowerPlacementInput() { Value = input, index = TowerIndex });
            //Debug.Log($"RaycastInput added to TowerPlacementInput buffer. Index={TowerIndex}");
        }
        else
        {
           // Debug.LogWarning("Ray did not hit the ground plane.");
        }
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

public struct TowerPlacementInput : IBufferElementData
{
    public RaycastInput Value;
    internal int index;
}
