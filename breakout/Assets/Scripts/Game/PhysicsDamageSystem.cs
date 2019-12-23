using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(StepPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
public class PhysicsDamageSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    private EndFramePhysicsSystem endFramePhysicsSystem;
    EntityQuery DamagerGroup;

    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();

        DamagerGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(PhysicsDamager) }
        });
    }

    [BurstCompile]
    struct CollisionEventDamageJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<PhysicsDamager> PhysicsDamagerGroup;
        public ComponentDataFromEntity<Health> HealthGroup;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entA = collisionEvent.Entities.EntityA;
            Entity entB = collisionEvent.Entities.EntityB;

            bool entAHasHealth = HealthGroup.Exists(entA);
            bool entBHasHealth = HealthGroup.Exists(entB);

            bool entAHasDamager = PhysicsDamagerGroup.Exists(entA);
            bool entBHasDamager = PhysicsDamagerGroup.Exists(entB);

            if (entAHasDamager && entBHasHealth)
            {
                ApplyDamage(entA, entB);
            }

            if (entBHasDamager && entAHasHealth)
            {
                ApplyDamage(entB, entA);
            }
        }

        private void ApplyDamage(Entity damager, Entity damaged)
        {
            PhysicsDamager damagerComp = PhysicsDamagerGroup[damager];
            Health healthComp = HealthGroup[damaged];

            healthComp.Value -= damagerComp.DamageDealtOnCollision;
            HealthGroup[damaged] = healthComp;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle jobHandle = new CollisionEventDamageJob
            {
                HealthGroup = GetComponentDataFromEntity<Health>(),
                PhysicsDamagerGroup = GetComponentDataFromEntity<PhysicsDamager>(true)
            }
            .Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);

        // todo understand if this is correct or how to avoid it
        // Right now, you can remove this BUT if there are no Health components around,
        // then DestroyDeadSystem doesn't run, which means nothing depends on this job
        // so nothing waits on it's completion, which leads to this job being left
        // outstanding until it collides with the physics systems next frame and causes read/write errors.

        // This at least ensures SOMETHING wants this to run so it doesn't hang around. Feels weird though.
        endFramePhysicsSystem.HandlesToWaitFor.Add(jobHandle);

        return jobHandle;
    }
}

