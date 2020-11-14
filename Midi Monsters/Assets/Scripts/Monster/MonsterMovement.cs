using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : MonoBehaviour
{
    [SerializeField]
    private float walkingSpeed = 2f;
    public float WalkingSpeed { get { return walkingSpeed; } }

    [SerializeField]
    private float runningSpeed = 4f;
    public float RunningSpeed { get { return runningSpeed; } }

    [SerializeField]
    private float chasingSpeed = 6f;
    public float ChasingSpeed { get { return chasingSpeed; } }

    private NavMeshAgent navMeshAgent;

    public Transform waypoint;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        MoveToPosition(waypoint.position, WalkingSpeed);
    }

    public void MoveToPosition(Vector3 position, float speed)
    {
        navMeshAgent.speed = speed;
        navMeshAgent.destination = position;
    }
}
