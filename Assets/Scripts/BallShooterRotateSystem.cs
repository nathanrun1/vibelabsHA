using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct BallShooterRotateSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RotateSpeed>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<LocalTransform> LocalTransform, RefRO<RotateSpeed> rotateSpeed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotateSpeed>>())
        {
            float curBallShooterMovementInput = SystemAPI.GetSingleton<Input>().ballShooterMovement;
            LocalTransform.ValueRW = LocalTransform.ValueRO.RotateZ(rotateSpeed.ValueRO.value * SystemAPI.Time.DeltaTime * curBallShooterMovementInput);
        }
    }
}
