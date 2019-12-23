using Unity.Entities;

[GenerateAuthoringComponent]
public struct PhysicsDamager : IComponentData
{
    public int DamageDealtOnCollision;
}