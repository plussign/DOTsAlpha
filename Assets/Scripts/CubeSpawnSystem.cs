using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// Run once: clone the baked renderable entity prefab 1000 times.
public partial struct CubeSpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CubeSpawner>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var settings = SystemAPI.GetSingleton<CubeSpawner>();
        var manager = state.EntityManager;
        var random = new Unity.Mathematics.Random(settings.Seed);

        float minX = -settings.Columns * settings.Step * 0.5f;
        float minZ = -settings.Rows * settings.Step * 0.5f;

        // One bulk instantiate call instead of 1000 structural-change calls.
        using (var instances = new NativeArray<Entity>(settings.Rows * settings.Columns, Allocator.Temp))
        {
            manager.Instantiate(settings.Prefab, instances);

            for (int i = 0; i < instances.Length; i++)
            {
                var entity = instances[i];
                var transform = manager.GetComponentData<LocalTransform>(entity);

                int row = i / settings.Columns;
                int column = i % settings.Columns;

                transform.Position = new float3(minX + column * settings.Step, 0f, minZ + row * settings.Step);
                manager.SetComponentData(entity, transform);

                uint seed = random.NextUInt();
                var entityRandom = new Unity.Mathematics.Random(seed == 0u ? 1u : seed);
                manager.SetComponentData(entity, new CubeMovement
                {
                    TimeToTurn = entityRandom.NextFloat(0.3f, 1.2f),
                    RotationAxis = entityRandom.NextFloat3Direction(),
                    RotationSpeed = entityRandom.NextFloat(settings.RoundSpeedRange.x, settings.RoundSpeedRange.y),
                    ColorChangeSpeed = settings.ColorChangeSpeed,
                    Rng = entityRandom
                });
            }
        }

        // Prevent another 1000 from spawning on the next frame.
        state.Enabled = false;
    }
}
