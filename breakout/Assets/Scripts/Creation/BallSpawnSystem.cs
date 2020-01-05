using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Authoring;
using Unity.Transforms;

public struct BallSpawnCommand : IComponentData
{
    public float2 Position;
    public float2 Velocity;
}

[AlwaysSynchronizeSystem]
public class BallSpawnSystem : JobComponentSystem
{
    private EntityCommandBufferSystem ecbSystem;
    private EntityQuery ballConfigQuery;
    private EntityQuery spawnQuery;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        
        ballConfigQuery = GetEntityQuery(ComponentType.ReadOnly<BallConfig>());
        RequireForUpdate(ballConfigQuery);
        
        // created by WithStoreEntityQueryInField magic
        RequireForUpdate(spawnQuery);
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        NativeArray<BallConfig> ballConfigs = ballConfigQuery.ToComponentDataArray<BallConfig>(Allocator.TempJob);
        
        Random random = new Random((uint)(Time.DeltaTime*1000));
    
        Entities
            .WithStoreEntityQueryInField(ref spawnQuery)
            .WithStructuralChanges()
            .ForEach((in BallSpawnCommand command) =>
        {
            BallConfig cfg = ballConfigs.GetRandom(random);

            LocalToWorld xform = EntityManager.GetComponentData<LocalToWorld>(cfg.Prefab);
            Entity newBall = EntityManager.Instantiate(cfg.Prefab);
            LimitDOFJoint.Create2D(EntityManager, newBall, new RigidTransform(xform.Value));

        }).Run();

        ballConfigs.Dispose();

        EntityManager.DestroyEntity(spawnQuery);

        return default;
    }
}

