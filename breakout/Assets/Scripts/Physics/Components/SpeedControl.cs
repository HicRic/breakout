using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpeedControl : IComponentData
{
    public float TargetSpeed;
    public float ControlPower;
}

