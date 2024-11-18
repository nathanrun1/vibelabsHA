using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI Score;
    public TextMeshProUGUI BallCount;
    private EntityManager entityManager;

    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    // Update is called once per frame
    void Update()
    {
        var query = entityManager.CreateEntityQuery(typeof(GameManager));
        var entities = query.ToEntityArray(Allocator.Temp);
        GameManager gameManager = entityManager.GetComponentData<GameManager>(entities[0]);
        Score.text = "Score: " + gameManager.score;
        BallCount.text = "Balls Shot: " + gameManager.ballsShot;
    }
}
