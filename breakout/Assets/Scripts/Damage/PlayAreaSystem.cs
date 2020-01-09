using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class PlayAreaSystem : JobComponentSystem
{
    private EntityCommandBufferSystem ecbSystem;
    private EntityQuery playAreaQuery;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        playAreaQuery = GetEntityQuery(ComponentType.ReadOnly<PlayArea>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var playAreas = playAreaQuery.ToComponentDataArray<PlayArea>(Allocator.TempJob);

        BoundsJob job = new BoundsJob
        {
            PlayAreas = playAreas,
            ECB = ecbSystem.CreateCommandBuffer().ToConcurrent()
        };

        JobHandle jobHandle = job.Schedule(this, inputDeps);
        ecbSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }

    [RequireComponentTag(typeof(DestroyOutsidePlayAreaTag))]
    private struct BoundsJob : IJobForEachWithEntity<Translation>
    {
        [DeallocateOnJobCompletion]
        public NativeArray<PlayArea> PlayAreas;

        public EntityCommandBuffer.Concurrent ECB;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation)
        {
            foreach (PlayArea playArea in PlayAreas)
            {
                // (Contains() is pure)
                // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                if (playArea.Value.Contains(translation.Value))
                {
                    return;
                }
            }

            ECB.DestroyEntity(index, entity);
        }
    }
}

