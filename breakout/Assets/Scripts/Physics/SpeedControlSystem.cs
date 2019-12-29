using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;

public class SpeedControlSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle job = Entities.ForEach((ref PhysicsVelocity velocity, in SpeedControl speedControl) =>
        {

        }).Schedule(inputDeps);

        return job;
    }
}

