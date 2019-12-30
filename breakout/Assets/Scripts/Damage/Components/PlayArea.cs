using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PlayArea : IComponentData
{
    public AABB Value;
}

