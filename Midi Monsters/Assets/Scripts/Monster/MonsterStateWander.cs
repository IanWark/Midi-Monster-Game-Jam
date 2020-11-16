using System;
using UnityEngine;

public class MonsterStateWander : MonoBehaviour
{
    [SerializeField, Tooltip("Point that we wander around. Can be the player.")]
    public Transform wanderPointCenter = null;

    [SerializeField, Tooltip("Max range from the wanderPointCenter we will wander to.")]
    public float wanderPointRadius = 40f;

    [SerializeField, Tooltip("Inclusive min in seconds how long the monster will wait after reaching a wander point.")]
    private float minWaitAtPoint = 0f;

    [SerializeField, Tooltip("Inclusive max in seconds how long the monster will wait after reaching a wander point.")]
    private float maxWaitAtPoint = 2f;

    MonsterMovement monsterMovement;

    private Vector3 currentTarget;
    private float waitTimer = 0;
    private float waitTimeTarget = 0;

    public void Awake()
    {
        monsterMovement = GetComponent<MonsterMovement>();
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
        if (wanderPointCenter != null && monsterMovement != null)
        {
            currentTarget = Monster.GetRandomNavmeshPoint(wanderPointCenter.position, wanderPointRadius);

            while (currentTarget == null || !monsterMovement.CanReachPosition(currentTarget))
            {
                Debug.Log("Can't reach position!");
                currentTarget = Monster.GetRandomNavmeshPoint(wanderPointCenter.position, wanderPointRadius);
            }

            monsterMovement.MoveToPosition(currentTarget, monsterMovement.WalkingSpeed);

            waitTimer = 0;
            waitTimeTarget = UnityEngine.Random.Range(minWaitAtPoint, maxWaitAtPoint);
        }
        else
        {
            Debug.Assert(wanderPointCenter != null, "wanderPointCenter != null");
            Debug.Assert(monsterMovement != null, "monsterMovement != null");
        }
    }
}
