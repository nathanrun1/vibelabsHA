using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;

public class ObstacleAuthoring : MonoBehaviour
{
    public int pointsValue = 10;
    public float afterLifeDuration = 1.5f; // Time after collide until remove this obstacle and score (removes anyway once all balls gone)
    public class Baker : Baker<ObstacleAuthoring>
    {
        public override void Bake(ObstacleAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Obstacle
            {
                collided = false,
                pointsValue = authoring.pointsValue,
                timeSinceCollide = 0f,
                afterLife = authoring.afterLifeDuration
            });
        }
    }
}

public struct Obstacle : IComponentData
{
    public bool collided;
    public int pointsValue;
    public float timeSinceCollide;
    public float afterLife;
}

public partial struct ObstacleScoreSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        //state.RequireForUpdate<Obstacle>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecs = new EntityCommandBuffer(Allocator.TempJob);
        foreach ((RefRW<Obstacle> obstacle, Entity entity) in SystemAPI.Query<RefRW<Obstacle>>().WithEntityAccess())
        {
            if (obstacle.ValueRO.collided)
                obstacle.ValueRW.timeSinceCollide += SystemAPI.Time.DeltaTime;
            if (obstacle.ValueRO.collided && (obstacle.ValueRW.timeSinceCollide > obstacle.ValueRO.afterLife || SystemAPI.GetSingleton<GameManager>().ballsInPlay == 0))
            {
                SystemAPI.GetSingletonRW<GameManager>().ValueRW.score += obstacle.ValueRO.pointsValue;
                ecs.DestroyEntity(entity);
            }
        }
        ecs.Playback(state.EntityManager);
        ecs.Dispose();
    }
}


