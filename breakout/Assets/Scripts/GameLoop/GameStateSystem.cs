using Unity.Entities;
using Unity.Jobs;

public class GameStateSystem : JobComponentSystem
{
    protected override void OnCreate()
    {
        Entity stateEntity = EntityManager.CreateEntity();
#if UNITY_EDITOR
        EntityManager.SetName(stateEntity, "GameState");
#endif

        GameState state = new GameState { CurrentPhase = GameState.Phase.Playing };
        EntityManager.AddComponentData(stateEntity, state);
        SetSingleton(state);

        LifeCount lifeCount = new LifeCount {Value = Config.Instance.StartingLives};
        EntityManager.AddComponentData(stateEntity, lifeCount);
        SetSingleton(lifeCount);
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return inputDeps;
    }
}

