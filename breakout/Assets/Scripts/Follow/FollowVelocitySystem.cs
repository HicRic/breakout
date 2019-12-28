using System.ComponentModel;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public class FollowVelocitySystem : JobComponentSystem
{
    private const float MinDistance = 0.5f;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        ComponentDataFromEntity<Translation> translationData = GetComponentDataFromEntity<Translation>();
        JobHandle followTargetJob = FollowTargetJob(inputDeps, translationData);
        return followTargetJob;
    }

    private JobHandle FollowTargetJob(JobHandle inputDeps, ComponentDataFromEntity<Translation> translationData)
    {
        JobHandle followTargetJob = Entities
            .WithReadOnly(translationData)
            .ForEach((ref PhysicsVelocity velocity, in FollowAxisX follow, in FollowTarget target, in Translation translation) =>
        {
            if (!translationData.Exists(target.Value))
            {
                return;
            }

            Translation targetTranslation = translationData[target.Value];
            float xAxisRelative = targetTranslation.Value.x - translation.Value.x;
            VelocityTowards(ref velocity, follow.Speed, xAxisRelative);

        }).Schedule(inputDeps);

        return followTargetJob;
    }
    
    private static void VelocityTowards(ref PhysicsVelocity velocity, float speed, float xAxisRelativePos)
    {
        float xVelocity = math.sign(xAxisRelativePos) * speed;
        float slowDown = math.saturate(math.abs(xAxisRelativePos) / MinDistance);
        xVelocity = math.lerp(0, xVelocity, slowDown);
        velocity.Linear = new float3(xVelocity, 0, 0);
    }
}

