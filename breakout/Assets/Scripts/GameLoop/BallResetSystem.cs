using Unity.Entities;
using Unity.Mathematics;

[AlwaysUpdateSystem]
[UpdateBefore(typeof(BallSpawnSystem))]
public class BallResetSystem : ComponentSystem
{
    private EntityQuery ballQuery;

    protected override void OnCreate()
    {
        ballQuery = GetEntityQuery(ComponentType.ReadOnly<BallTag>());
    }

    protected override void OnUpdate()
    {
        // If query is empty, a ball exists, so we don't need to spawn one.
        if (!ballQuery.IsEmptyIgnoreFilter)
        {
            return;
        }

        Entity request = EntityManager.CreateEntity();
        
        EntityManager.AddComponentData(request, new BallSpawnCommand
        {
            Position = new float2(0, -1f),
            Velocity = new float2(0, -9f)
        });
    }
}

