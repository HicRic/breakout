using Unity.Entities;
using Unity.Jobs;

[UpdateAfter(typeof(PhysicsDamageSystem))]
public class DestroyDeadSystem : JobComponentSystem
{
    private EntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer.Concurrent buffer = ecbSystem.CreateCommandBuffer().ToConcurrent();

        var job = Entities.ForEach((int entityInQueryIndex, ref Entity entity, in Health health) =>
        {
            if (health.Value <= 0)
            {
                buffer.DestroyEntity(entityInQueryIndex, entity);
            }

        }).Schedule(inputDeps);

        ecbSystem.AddJobHandleForProducer(job);

        return job;
    }
}

