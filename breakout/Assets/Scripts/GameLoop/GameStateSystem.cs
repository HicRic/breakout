using Unity.Entities;
using Unity.Jobs;

public struct ResetLevelCommand : IComponentData { }

public class GameStateSystem : JobComponentSystem
{
    private EntityQuery resetQuery;

    protected override void OnCreate()
    {
        Entity stateEntity = EntityManager.CreateEntity();
#if UNITY_EDITOR
        EntityManager.SetName(stateEntity, "GameState");
#endif

        GameState state = new GameState { CurrentPhase = GameState.Phase.Playing };
        EntityManager.AddComponentData(stateEntity, state);
        SetSingleton(state);

        LifeCount lifeCount = new LifeCount { Value = Config.Instance.StartingLives };
        EntityManager.AddComponentData(stateEntity, lifeCount);
        SetSingleton(lifeCount);

        resetQuery = GetEntityQuery(typeof(ResetLevelCommand));
    }

    private void DoLevelReset()
    {
        SetSingleton(new GameState { CurrentPhase = GameState.Phase.Playing });
        SetSingleton(new LifeCount { Value = Config.Instance.StartingLives });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (!resetQuery.IsEmptyIgnoreFilter)
        {
            DoLevelReset();
            EntityManager.DestroyEntity(resetQuery);
        }

        return inputDeps;
    }
}

