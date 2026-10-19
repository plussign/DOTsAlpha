using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
public partial struct CubeMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CubeMovement>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new MoveJob { DeltaTime = SystemAPI.Time.DeltaTime };
        state.Dependency = job.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    public partial struct MoveJob : IJobEntity
    {
        public float DeltaTime;

        public void Execute(ref LocalTransform transform, ref CubeMovement movement, ref URPMaterialPropertyBaseColor baseColor)
        {
            var random = movement.Rng;
            movement.TimeToTurn -= DeltaTime;

            if (movement.TimeToTurn <= 0f)
            {
                //movement.Velocity = random.NextFloat3Direction() * random.NextFloat(1f, 4f);
                movement.TimeToTurn = random.NextFloat(0.3f, 1.2f);
            }

            if (movement.ColorTransitionDuration <= 0f ||
                movement.ColorTransitionElapsed >= movement.ColorTransitionDuration)
            {
                // Begin each transition at the current color, including the baked initial color.
                movement.ColorStart = baseColor.Value;
                movement.ColorTarget = new float4(random.NextFloat3(), 1f);
                movement.ColorTransitionDuration = random.NextFloat(2f, 4f);
                movement.ColorTransitionElapsed = 0f;
            }

            movement.ColorTransitionElapsed = math.min(
                movement.ColorTransitionElapsed + DeltaTime * movement.ColorChangeSpeed,
                movement.ColorTransitionDuration);
            float colorProgress = movement.ColorTransitionElapsed / movement.ColorTransitionDuration;
            // Ease in and out so changing targets also feels smooth.
            baseColor.Value = math.lerp(movement.ColorStart, movement.ColorTarget,
                math.smoothstep(0f, 1f, colorProgress));

            /*float3 position = transform.Position + movement.Velocity * DeltaTime;
            float extent = movement.HalfExtent;

            // Bounce off the six sides of the invisible simulation volume.
            if (math.abs(position.x) > extent)
            {
                position.x = math.clamp(position.x, -extent, extent);
                movement.Velocity.x = -movement.Velocity.x;
            }
            if (math.abs(position.y) > extent)
            {
                position.y = math.clamp(position.y, -extent, extent);
                movement.Velocity.y = -movement.Velocity.y;
            }
            if (math.abs(position.z) > extent)
            {
                position.z = math.clamp(position.z, -extent, extent);
                movement.Velocity.z = -movement.Velocity.z;
            }*/

            //transform.Position = position;
            //float3 axis = movement.RotationAxis.value.xyz;
            //movement.RotationAxis.value.w += DeltaTime % 2f - 1f;
            
            transform.Rotation = math.mul(
                transform.Rotation, 
                quaternion.AxisAngle(movement.RotationAxis, movement.RotationSpeed * DeltaTime));
            movement.Rng = random;
        }
    }
}
