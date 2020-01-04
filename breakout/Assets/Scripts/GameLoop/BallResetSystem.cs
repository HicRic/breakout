using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Authoring;
using Unity.Transforms;

public class BallResetSystem : JobComponentSystem
{
    private EntityCommandBufferSystem ecbSystem;
    private EntityQuery ballQuery;
    private EntityQuery ballConfigQuery;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        ballQuery = GetEntityQuery(ComponentType.ReadOnly<BallTag>());
        ballConfigQuery = GetEntityQuery(ComponentType.ReadOnly<BallConfig>());
        RequireForUpdate(ballConfigQuery);
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (!ballQuery.IsEmptyIgnoreFilter)
        {
            return inputDeps;
        }

        using (NativeArray<BallConfig> ballConfigs = ballConfigQuery.ToComponentDataArray<BallConfig>(Allocator.TempJob))
        {
            Random random = new Random((uint)(Time.DeltaTime*1000));
            EntityCommandBuffer buffer = ecbSystem.CreateCommandBuffer();
            BallConfig cfg = ballConfigs.GetRandom(random);

            LocalToWorld xform = EntityManager.GetComponentData<LocalToWorld>(cfg.Prefab);
            Entity newBall = buffer.Instantiate(cfg.Prefab);
            LimitDOFJoint.Create2D(buffer, newBall, new RigidTransform(xform.Value));
        }

        return inputDeps;
    }
}

