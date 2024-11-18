using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GameManagerAuthoring : MonoBehaviour
{
    public class Baker : Baker<GameManagerAuthoring>
    {
        public override void Bake(GameManagerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GameManager
            {
                score = 0,
                ballsInPlay = 0,
                ballsShot = 0
            });
        }
    }
}

public struct GameManager : IComponentData
{
    public int score; // Current player score
    public int ballsInPlay; // Amount of balls currently "in play" on the screen. Player can't shoot if this value exceeds 0.
    public int ballsShot;
}

public enum CollisionLayers
{
    Default = 1 << 0,
    Ball = 1 << 6,
    Obstacle = 1 << 7
}
