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

        DamagerQuery = GetEntityQuery(ComponentType.ReadOnly<PhysicsDamager>());
        RequireForUpdate(DamagerQuery);
        HealthQuery = GetEntityQuery(ComponentType.ReadOnly<Health>());
        RequireForUpdate(HealthQuery);
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
        JobHandle jobHandle = new CollisionEventDamageJob
        {
            HealthGroup = GetComponentDataFromEntity<Health>(),
            PhysicsDamagerGroup = GetComponentDataFromEntity<PhysicsDamager>(true)
        }
        .Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);

        return jobHandle;
    }
}

