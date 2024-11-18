using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class InputAuthoring : MonoBehaviour
{
    private class Baker : Baker<InputAuthoring>
    {
        public override void Bake(InputAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Input
            {
                ballShooterMovement = 0,
                ballShoot = false
            });
        }
    }
}

public struct Input : IComponentData
{
    public float ballShooterMovement;
    public bool ballShoot;
}