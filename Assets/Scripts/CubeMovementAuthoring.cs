using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

// Per-entity runtime state: no MonoBehaviour Update() is involved.
public struct CubeMovement : IComponentData
{
    //public float3 Velocity;
    public float TimeToTurn;
    public float3 RotationAxis;
    public float RotationSpeed;
    public float ColorChangeSpeed;
    public float4 ColorStart;
    public float4 ColorTarget;
    public float ColorTransitionDuration;
    public float ColorTransitionElapsed;
    public Unity.Mathematics.Random Rng;
}

// Add this authoring component to the Cube prefab.
public class CubeMovementAuthoring : MonoBehaviour
{
}

public class CubeMovementBaker : Baker<CubeMovementAuthoring>
{
    public override void Bake(CubeMovementAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new CubeMovement
        {
            TimeToTurn = 0f,
            RotationAxis = float3.zero,
            RotationSpeed = 0f,
            ColorChangeSpeed = 1f,
            Rng = new Unity.Mathematics.Random(1u)
        });

        // The prefab's renderer is on this same GameObject.
        var meshRenderer = GetComponent<MeshRenderer>();
        var material = meshRenderer != null ? meshRenderer.sharedMaterial : null;
        DependsOn(material);
        var color = material != null && material.HasProperty("_BaseColor")
            ? material.GetColor("_BaseColor").linear
            : Color.white;
        AddComponent(entity, new URPMaterialPropertyBaseColor
        {
            Value = new float4(color.r, color.g, color.b, color.a)
        });
    }
}
