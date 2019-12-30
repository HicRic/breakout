using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class PlayAreaSystem : JobComponentSystem
{
    private EntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var playAreas = GetEntityQuery(typeof(PlayArea)).ToComponentDataArray<PlayArea>(Allocator.TempJob);

        BoundsJob job = new BoundsJob
        {
            PlayAreas = playAreas,
            ECB = ecbSystem.CreateCommandBuffer().ToConcurrent()
        };

        return job.Schedule(this, inputDeps);
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

