
using Unity.Entities;

public struct Enemy : IComponentData
{
    public float speed;
    public Entity target;
    public float health;
}