using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;

public class BallDeathAuthoring : MonoBehaviour
{
    public class Baker : Baker<BallDeathAuthoring>
    {
        public override void Bake(BallDeathAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BallDeathTrigger());
        }
    }
}

public struct BallDeathTrigger : IComponentData { }

[BurstCompile]
public partial struct BallDeathSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BallDeathTrigger>();
    }

    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecs = new EntityCommandBuffer(Allocator.TempJob);
        foreach (RefRO<LocalToWorld> localToWorld in SystemAPI.Query<RefRO<LocalToWorld>>().WithAll<BallDeathTrigger>())
        {
            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);

            physicsWorld.BoxCastAll(localToWorld.ValueRO.Position, Quaternion.identity, new float3(100f, 2f, 100f),
                float3.zero, 1f, ref hits, new CollisionFilter { BelongsTo = (uint)CollisionLayers.Default, CollidesWith = (uint)CollisionLayers.Ball });

            foreach(ColliderCastHit hit in hits)
            {
                if (SystemAPI.HasComponent<BallTag>(hit.Entity))
                {                    
                    ecs.DestroyEntity(hit.Entity);                 
                    SystemAPI.GetSingletonRW<GameManager>().ValueRW.ballsInPlay -= 1;
                }
            }
        }
        ecs.Playback(state.EntityManager);
        ecs.Dispose();
    }
}
