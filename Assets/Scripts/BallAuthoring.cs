using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class BallAuthoring : MonoBehaviour
{
    public class Baker : Baker<BallAuthoring>
    {
        public override void Bake(BallAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BallTag());         
        }
    }
}

public struct BallTag : IComponentData { }


[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct BallCollideSystem : ISystem
{
    private ComponentLookup<BallTag> ballLookup;
    private ComponentLookup<Obstacle> obstacleLookup;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Obstacle>();
        state.RequireForUpdate<BallTag>();

        ballLookup = SystemAPI.GetComponentLookup<BallTag>(true);
        obstacleLookup = SystemAPI.GetComponentLookup<Obstacle>(false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<LocalTransform> localTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<BallTag>())
        {
            localTransform.ValueRW.Position = new float3(localTransform.ValueRO.Position.x, localTransform.ValueRO.Position.y, 0);
        }

        SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();

        ballLookup.Update(ref state);
        obstacleLookup.Update(ref state);
        state.Dependency = new BallCollideJob()
        {
            Balls = ballLookup,
            Obstacles = obstacleLookup
        }.Schedule(simulation, state.Dependency);
    }
}

[BurstCompile]
public struct BallCollideJob : ICollisionEventsJob
{
    [ReadOnly] public ComponentLookup<BallTag> Balls;
    public ComponentLookup<Obstacle> Obstacles;

    public void Execute(CollisionEvent collisionEvent)
    {
        Entity ball = Entity.Null;
        Entity obstacle = Entity.Null;

        Debug.Log("trigger");

        if (Balls.HasComponent(collisionEvent.EntityA))
            ball = collisionEvent.EntityA;
        if (Balls.HasComponent(collisionEvent.EntityB))
            ball = collisionEvent.EntityB;
        if (Obstacles.HasComponent(collisionEvent.EntityA))
            obstacle = collisionEvent.EntityA;
        if (Obstacles.HasComponent(collisionEvent.EntityB))
            obstacle = collisionEvent.EntityB;

        if (Entity.Null.Equals(ball) || Entity.Null.Equals(obstacle)) return;

        Debug.Log("hit");

        Obstacle obstacleComp = Obstacles[obstacle];
        obstacleComp.collided = true;
        Obstacles[obstacle] = obstacleComp;
    }
}