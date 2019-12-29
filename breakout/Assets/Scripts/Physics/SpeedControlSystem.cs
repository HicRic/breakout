using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateBefore(typeof(BuildPhysicsWorld))]
public class SpeedControlSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = World.Time.fixedDeltaTime;

        JobHandle job = Entities.ForEach((ref PhysicsVelocity velocity, in SpeedControl speedControl) =>
        {
            float currentSpeedSq = math.lengthsq(velocity.Linear);
            float targetSpeedSq = speedControl.TargetSpeedSq;

            // How far away from the desired speed are we?
            float desiredSpeedSqDelta = targetSpeedSq - currentSpeedSq;

            // What's the most we can change?
            float maxSpeedSqDeltaThisFrame = desiredSpeedSqDelta * speedControl.ControlPower * deltaTime;

            // Don't change more than the total desired change though - avoid overshooting.
            float speedSqDeltaThisFrame = math.min(math.abs(maxSpeedSqDeltaThisFrame), math.abs(desiredSpeedSqDelta));

            float speedDeltaThisFrame = math.sqrt(speedSqDeltaThisFrame);
            velocity.Linear += math.normalizesafe(velocity.Linear) * speedDeltaThisFrame * math.sign(desiredSpeedSqDelta);

        }).Schedule(inputDeps);

        return job;
    }
}

