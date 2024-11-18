using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine;

public class BallMultiplierAuthoring : MonoBehaviour
{
    public int ballAmount = 5;
    public class Baker : Baker<BallMultiplierAuthoring>
    {
        public override void Bake(BallMultiplierAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BallMultiplier
            {
                ballAmount = authoring.ballAmount
            });
        }
    }
}

public struct BallMultiplier : IComponentData
{
    public int ballAmount;
}

public partial struct BallMultiplierSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BallMultiplier>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecs = new EntityCommandBuffer(Allocator.TempJob);
        foreach ((RefRO<Obstacle> obstacle, RefRO<BallMultiplier> ballMultiplier, RefRO<LocalToWorld> localToWorld, Entity entity) 
            in SystemAPI.Query<RefRO<Obstacle>, RefRO<BallMultiplier>, RefRO<LocalToWorld>>().WithEntityAccess())
        {
            if (obstacle.ValueRO.collided)
            {
                SystemAPI.GetSingletonRW<GameManager>().ValueRW.score += obstacle.ValueRO.pointsValue;
                ecs.DestroyEntity(entity);

                RefRW<GameManager> gameManager = SystemAPI.GetSingletonRW<GameManager>();
                
                for (int i = 0; i < ballMultiplier.ValueRO.ballAmount; ++i)
                {

                    Entity spawnedBall = state.EntityManager.Instantiate(SystemAPI.GetSingleton<BallShoot>().ballPrefabEntity);
                    state.EntityManager.SetComponentData(spawnedBall, new LocalTransform
                    {
                        Position = localToWorld.ValueRO.Position,
                        Scale = SystemAPI.GetSingleton<BallShoot>().ballScale,
                        Rotation = Quaternion.identity
                    });
                    state.EntityManager.SetComponentData(spawnedBall, new PhysicsVelocity
                    {
                        Linear = math.normalize(new float3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0)) * 5
                    });
                }
                gameManager.ValueRW.ballsInPlay += ballMultiplier.ValueRO.ballAmount;
            }
        }
        ecs.Playback(state.EntityManager);
        ecs.Dispose();
    }
}
