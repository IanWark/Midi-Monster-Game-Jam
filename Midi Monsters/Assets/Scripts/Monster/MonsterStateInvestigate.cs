using System;
using UnityEngine;

public class MonsterStateInvestigate : MonoBehaviour
{
    public event Action OnExitState;

    [SerializeField, Tooltip("Inclusive min in seconds how long the monster will wait after reaching a investigate point.")]
    private float minWaitAtPoint = 0f;

    [SerializeField, Tooltip("Inclusive max in seconds how long the monster will wait after reaching a investigate point.")]
    private float maxWaitAtPoint = 1f;

    [SerializeField, Tooltip("How far away from the last sound we heard we will search.")]
    public float investigateRadius = 10f;

    [SerializeField, Tooltip("How many random points we will search.")]
    private int investigatePoints = 3;

    private Monster monster;
    private MonsterMovement monsterMovement;

    private Vector3 soundPoint;
    private Vector3 currentTarget;
    private int pointsSearched = 0;

    private float waitTimer = 0;
    private float waitTimeTarget = 0;

    public void Start()
    {
        monster = GetComponent<Monster>();
        monsterMovement = GetComponent<MonsterMovement>();
    }

    public void EnterState(Monster.DetectedSound detectedSound)
    {
        pointsSearched = 0;
        soundPoint = detectedSound.position;
        currentTarget = soundPoint;

        GetNewTargetPoint();
    }

    public void Tick()
    {
        waitTimer += Time.deltaTime;

        if (pointsSearched >= investigatePoints)
        {
            OnExitState?.Invoke();
        }
        else if (monsterMovement.IsAtDestination() && waitTimer >= waitTimeTarget)
        {
            pointsSearched += 1;
            GetNewTargetPoint();
        }
    }

    private void GetNewTargetPoint()
    {
        currentTarget = Monster.GetRandomNavmeshPoint(soundPoint, investigateRadius);

        monsterMovement.MoveToPosition(currentTarget, monsterMovement.WalkingSpeed);

        waitTimer = 0;
        waitTimeTarget = UnityEngine.Random.Range(minWaitAtPoint, maxWaitAtPoint);
    }
}
