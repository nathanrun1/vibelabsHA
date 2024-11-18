using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
public partial class GetInputSystem : SystemBase
{
    private BallShooterInput _ballShooterInput;
    protected override void OnCreate()
    {
        RequireForUpdate<Input>();

        _ballShooterInput = new BallShooterInput();
    }

    protected override void OnStartRunning()
    {
        _ballShooterInput.Enable();
    }

    protected override void OnUpdate()
    {
        float curBallShooterMovementInput = _ballShooterInput.ActionMap.BallShooterMove.ReadValue<float>();
        var curBallShootInput = _ballShooterInput.ActionMap.BallShooterShoot.WasPressedThisFrame();
        SystemAPI.SetSingleton(new Input
        {
            ballShooterMovement = curBallShooterMovementInput,
            ballShoot = curBallShootInput
        });
    }

    protected override void OnStopRunning()
    {
        _ballShooterInput.Disable();
    }
}
