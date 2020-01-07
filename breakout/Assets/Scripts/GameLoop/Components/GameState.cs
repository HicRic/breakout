using Unity.Entities;

public struct GameState : IComponentData
{
    public enum Phase
    {
        Playing,
        Won,
        Lost
    }

    public Phase CurrentPhase;
}

