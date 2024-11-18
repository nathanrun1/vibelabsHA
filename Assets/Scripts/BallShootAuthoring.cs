using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Unity.Physics;

public class BallShootAuthoring : MonoBehaviour
{
    public GameObject ballPrefab;
    public float ballInitialVelocity = 7f;
    public float ballScale = 0.3f;

    public class Baker : Baker<BallShootAuthoring>
    {
        public override void Bake(BallShootAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BallShoot
            {
                parent = entity,
                ballPrefabEntity = GetEntity(authoring.ballPrefab, TransformUsageFlags.Dynamic),
                ballInitialVelocity = authoring.ballInitialVelocity,
                ballScale = authoring.ballScale
            });
            /*AddComponent(entity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Scale = 0.2f,
                Rotation = Quaternion.identity
            });*/
        }
    }
}

public struct BallShoot : IComponentData
{
    public Entity parent;
    public Entity ballPrefabEntity;
    public float ballInitialVelocity;
    public float ballScale;
}

public readonly partial struct BallShootAspect : IAspect
{
    public readonly RefRO<BallShoot> ballShoot;
    public readonly RefRO<LocalToWorld> localToWorld;
}

public partial struct BallShootSystem : ISystem
{
    EntityManager entityManager;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BallShoot>();
        state.RequireForUpdate<Input>();
        entityManager = state.EntityManager;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (BallShootAspect ballShootAspect in SystemAPI.Query<BallShootAspect>())
        {
            if (SystemAPI.GetSingleton<Input>().ballShoot && SystemAPI.GetSingleton<GameManager>().ballsInPlay == 0)
            {
                RefRW<GameManager> gameManager = SystemAPI.GetSingletonRW<GameManager>();
                gameManager.ValueRW.ballsInPlay += 1;
                gameManager.ValueRW.ballsShot += 1;

                Entity spawnedBall = entityManager.Instantiate(ballShootAspect.ballShoot.ValueRO.ballPrefabEntity);
                Entity ballShootParent = entityManager.GetComponentData<Parent>(ballShootAspect.ballShoot.ValueRO.parent).Value;
                float3 ballShootParentDown = entityManager.GetComponentData<LocalTransform>(ballShootParent).Up() * -1;

                entityManager.SetComponentData(spawnedBall, new LocalTransform
                {
                    Position = ballShootAspect.localToWorld.ValueRO.Position,
                    Scale = ballShootAspect.ballShoot.ValueRO.ballScale,
                    Rotation = Quaternion.identity
                });
                entityManager.SetComponentData(spawnedBall, new PhysicsVelocity
                {
                    Linear = ballShootAspect.ballShoot.ValueRO.ballInitialVelocity * ballShootParentDown
                });
            }
        }
    }
}

