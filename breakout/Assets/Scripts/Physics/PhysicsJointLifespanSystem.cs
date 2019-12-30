using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;

public struct PhysicsJointEntityTag : IComponentData{}

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class PhysicsJointLifespanSystem : JobComponentSystem
{
    private EntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer.Concurrent buffer = ecbSystem.CreateCommandBuffer().ToConcurrent();
        ComponentDataFromEntity<PhysicsJointEntityTag> taggedEntityData = GetComponentDataFromEntity<PhysicsJointEntityTag>();

        JobHandle job = Entities
            .WithReadOnly(taggedEntityData)
            .ForEach((int entityInQueryIndex, ref Entity entity, in PhysicsJoint joint) =>
            {
                if (!taggedEntityData.Exists(joint.EntityA))
                {
                    buffer.DestroyEntity(entityInQueryIndex, entity);
                }

            }).Schedule(inputDeps);

        ecbSystem.AddJobHandleForProducer(job);

        return job;
    }
}

