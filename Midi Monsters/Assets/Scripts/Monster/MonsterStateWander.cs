using System;
using UnityEngine;

public class MonsterStateWander : MonoBehaviour
{
    [SerializeField, Tooltip("Max range from the player we will wander to.")]
    public float wanderPointFromPlayerRadius = 40f;

    [SerializeField, Tooltip("Inclusive min in seconds how long the monster will wait after reaching a wander point.")]
    private float minWaitAtPoint = 0f;

    [SerializeField, Tooltip("Inclusive max in seconds how long the monster will wait after reaching a wander point.")]
    private float maxWaitAtPoint = 2f;

    Monster monster;
    MonsterMovement monsterMovement;
    PlayerCharacterController player;

    private Vector3 currentTarget;
    private float waitTimer = 0;
    private float waitTimeTarget = 0;

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
        waitTimer += Time.deltaTime;

        if (monsterMovement.IsAtDestination() && waitTimer >= waitTimeTarget)
        {
            GetNewTargetPoint();
        }
    }

    private void GetNewTargetPoint()
    {
        if (player != null)
        {
            currentTarget = Monster.GetRandomNavmeshPoint(player.transform.position, wanderPointFromPlayerRadius);

            monsterMovement.MoveToPosition(currentTarget, monsterMovement.WalkingSpeed);

            waitTimer = 0;
            waitTimeTarget = UnityEngine.Random.Range(minWaitAtPoint, maxWaitAtPoint);
        }
    }
}
