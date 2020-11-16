using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField]
    public float walkingSpeed = 2f;
    public float WalkingSpeed { get { return walkingSpeed; } }

    [SerializeField]
    public float runningSpeed = 4f;
    public float RunningSpeed { get { return runningSpeed; } }

    [SerializeField]
    public float sprintingSpeed = 6f;
    public float SprintingSpeed { get { return sprintingSpeed; } }

    [Header("References")]
    [SerializeField]
    private Transform debugCurrentDestination;

    private NavMeshAgent navMeshAgent;

    public float GetAgentSpeed() { return navMeshAgent.speed; }

    // Start is called before the first frame update
    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void MoveToPosition(Vector3 position, float speed)
    {
        if (navMeshAgent.isActiveAndEnabled)
        {
            navMeshAgent.speed = speed;
            navMeshAgent.destination = position;

            debugCurrentDestination.position = position;
        }
    }

    public bool IsAtDestination()
    {
        return !navMeshAgent.pathPending && navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance;
    }

    public bool CanReachPosition(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        navMeshAgent.CalculatePath(position, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }
}
