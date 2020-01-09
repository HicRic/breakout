using Unity.Entities;
using Unity.Mathematics;

[AlwaysUpdateSystem]
[UpdateBefore(typeof(BallSpawnSystem))]
public class LivesRespawnBallsSystem : ComponentSystem
{
    private EntityQuery ballQuery;

    protected override void OnCreate()
    {
        ballQuery = GetEntityQuery(ComponentType.ReadOnly<BallTag>());
    }

    protected override void OnUpdate()
    {
        //todo this system does too many things I think

        // ideally we use RequireSingletonForUpdate but we want to cache an entity query
        // to avoid GC alloc, but doing so means I also need [AlwaysUpdateSystem]
        if (!HasSingleton<LifeCount>() || !HasSingleton<GameState>())
        {
            return;
        }

        // If query is empty, a ball exists, so we don't need to spawn one.
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

