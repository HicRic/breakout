using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpeedControl : IComponentData
{
    public float TargetSpeedSq;
    public float ControlPower;
}

