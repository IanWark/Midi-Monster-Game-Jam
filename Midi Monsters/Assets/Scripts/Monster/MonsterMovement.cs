﻿using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField]
    private float walkingSpeed = 2f;
    public float WalkingSpeed { get { return walkingSpeed; } }

    [SerializeField]
    private float runningSpeed = 4f;
    public float RunningSpeed { get { return runningSpeed; } }

    [SerializeField]
    private float sprintingSpeed = 6f;
    public float SprintingSpeed { get { return sprintingSpeed; } }

    [Header("References")]
    [SerializeField]
    private Transform debugCurrentDestination;

    private NavMeshAgent navMeshAgent;

    public float GetAgentSpeed() { return navMeshAgent.speed; }

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void MoveToPosition(Vector3 position, float speed)
    {
        navMeshAgent.speed = speed;
        navMeshAgent.destination = position;

        debugCurrentDestination.position = position;
    }

    public bool IsAtDestination()
    {
        return !navMeshAgent.pathPending && navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance;
    }
}
