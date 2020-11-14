using System;
using UnityEngine;

public class MonsterStateWander : MonoBehaviour
{
    [SerializeField, Tooltip("Max range from the player we will wander to.")]
    private float wanderPointFromPlayerRadius = 40f;

    Monster monster;
    MonsterMovement monsterMovement;
    PlayerCharacterController player;

    private Vector3 currentTarget;

    public void Start()
    {
        monster = GetComponent<Monster>();
        monsterMovement = GetComponent<MonsterMovement>();
        player = FindObjectOfType<PlayerCharacterController>();
    }

    public void EnterState()
    {
        GetNewTargetPoint();
    }

    public void Tick()
    {
        if (monsterMovement.IsAtDestination())
        {
            GetNewTargetPoint();
        }
    }

    private void GetNewTargetPoint()
    {
        currentTarget = Monster.GetRandomNavmeshPoint(player.transform.position, wanderPointFromPlayerRadius);

        monsterMovement.MoveToPosition(currentTarget, monsterMovement.WalkingSpeed);
    }
}
