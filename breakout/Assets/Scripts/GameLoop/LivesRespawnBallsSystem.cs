using Unity.Entities;
using Unity.Mathematics;

[UpdateBefore(typeof(BallSpawnSystem))]
public class LivesRespawnBallsSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<LifeCount>();
        RequireSingletonForUpdate<GameState>();
    }

    protected override void OnUpdate()
    {
        //todo this system does too many things I think

        // If query is empty, a ball exists, so we don't need to spawn one.
        EntityQuery ballQuery = GetEntityQuery(ComponentType.ReadOnly<BallTag>());
        if (!ballQuery.IsEmptyIgnoreFilter)
        {
            return;
        }

        LifeCount lifeCount = GetSingleton<LifeCount>();
        if (lifeCount.Value <= 0)
        {
            GameState state = GetSingleton<GameState>();
            state.CurrentPhase = GameState.Phase.Lost;
            SetSingleton(state);
            return;
        }

        lifeCount.Value--;
        SetSingleton(lifeCount);

        Entity request = EntityManager.CreateEntity();
        
        EntityManager.AddComponentData(request, new BallSpawnCommand
        {
            Position = new float2(0, -1f),
            Velocity = new float2(0, -9f)
        });
    }
}

