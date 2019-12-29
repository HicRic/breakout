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
    private EntityQuery DamagerQuery;
    private EntityQuery HealthQuery;

    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();

        DamagerQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new [] { ComponentType.ReadOnly<PhysicsDamager>() }
        });

        HealthQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(Health) }
        });
    }

    [BurstCompile]
    private struct CollisionEventDamageJob : ICollisionEventsJob
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
        // todo more elegant way of specifying "run system if there are Health AND PhysicsDamager components"
        // (entity query takes care of one but not other...right?)
        // If we schedule the job when there's no health components left,
        // we do no required work and our job is never .Completed() and causes errors next frame
        int healthCount = HealthQuery.CalculateEntityCount();
        if (healthCount == 0)
        {
            return inputDeps;
        }

        JobHandle jobHandle = new CollisionEventDamageJob
        {
            HealthGroup = GetComponentDataFromEntity<Health>(),
            PhysicsDamagerGroup = GetComponentDataFromEntity<PhysicsDamager>(true)
        }
        .Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);

        return jobHandle;
    }
}

