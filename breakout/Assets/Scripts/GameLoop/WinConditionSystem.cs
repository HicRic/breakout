using System;
using Unity.Entities;
using Unity.Jobs;

[AlwaysSynchronizeSystem]
public class WinConditionSystem : JobComponentSystem
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameState>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        // later on we'll have objective blocks that need to be destroyed
        // and tag them as such - right now everything that has health fine though.
        var healthObjectsQuery = GetEntityQuery(ComponentType.ReadOnly<Health>());
        if (healthObjectsQuery.IsEmptyIgnoreFilter)
        {
            // we require this singleton for update, so no need to worry if it exists or anything
            GameState state = GetSingleton<GameState>();
            state.CurrentPhase = GameState.Phase.Won;
            SetSingleton(state);
        }

        return default;
    }
}

