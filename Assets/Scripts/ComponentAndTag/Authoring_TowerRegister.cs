using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Towers : IBufferElementData
{
    public Entity Prefab;  // 存储塔的实体
}

public class Authoring_TowerRegister : MonoBehaviour
{
    public List<GameObject> Towers;

    class Baker : Baker<Authoring_TowerRegister>
    {
        public override void Bake(Authoring_TowerRegister authoring)
        {
            // 获取实体
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // 为实体添加一个缓冲区来存储塔的实体
            var buffer = AddBuffer<Towers>(entity);  // 传递entity参数

            // 将每个塔预制体转换为实体并添加到缓冲区中
            foreach (var tower in authoring.Towers)
            {
                buffer.Add(new Towers { Prefab = GetEntity(tower, TransformUsageFlags.Dynamic) });
            }
        }
    }
}
