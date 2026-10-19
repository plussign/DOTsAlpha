using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public struct CubeSpawner : IComponentData
{
    public Entity Prefab;
    public int Rows;
    public int Columns;
    public float Step;
    public float2 RoundSpeedRange;
    public float ColorChangeSpeed;
    public uint Seed;
}

// Put this GameObject inside a SubScene. Assign the Cube prefab.
public class CubeSpawnerAuthoring : MonoBehaviour
{
    public GameObject CubePrefab;
    [Min(1)] public int Rows = 10;
    [Min(1)] public int Columns = 10;
    [Min(1)] public float Step = 2f;
    public Vector2 RoundSpeedRange = new Vector2(-1f, 1f);
    [Min(0f)]
    [Tooltip("Color transition speed multiplier: 1 = 2-4 seconds, 2 = 1-2 seconds, 0 = paused.")]
    public float ColorChangeSpeed = 2f;
    public int Seed = 12345;
}

public class CubeSpawnerBaker : Baker<CubeSpawnerAuthoring>
{
    public override void Bake(CubeSpawnerAuthoring authoring)
    {
        if (authoring.CubePrefab == null)
            return;

        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new CubeSpawner
        {
            Prefab      = GetEntity(authoring.CubePrefab, TransformUsageFlags.Dynamic),
            Rows        = Mathf.Max(1, authoring.Rows),
            Columns     = Mathf.Max(1, authoring.Columns),
            Step        = Mathf.Max(1f, authoring.Step),
            RoundSpeedRange = new float2(authoring.RoundSpeedRange.x, authoring.RoundSpeedRange.y),
            ColorChangeSpeed = Mathf.Max(0f, authoring.ColorChangeSpeed),
            Seed        = (uint)Mathf.Max(1, authoring.Seed)
        });
    }
}
