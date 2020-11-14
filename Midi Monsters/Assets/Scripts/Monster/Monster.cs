using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public class DetectedSound
    {
        public DetectedSound(Vector3 position, Vector3 predictedPosition)
        {
            this.position = position;
            this.predictedPosition = predictedPosition;
        }

        public Vector3 position;
        public Vector3 predictedPosition;
    }

    public enum eMonsterState
    {
        SprintToSound = 0,
        GoToSound = 1,
        Investigate = 2,
        Wander = 3,
    }

    [SerializeField]
    private Transform startWaypoint;
    
    private eMonsterState currentState = eMonsterState.Wander;
    public eMonsterState CurrentState { get { return currentState; } }
    private DetectedSound lastDetectedSound = null;

    MonsterStateGoToSound monsterStateGoToSound;
    MonsterStateInvestigate monsterStateInvestigatePoint;
    MonsterStateWander monsterStateWander;

    MonsterMovement monsterMovement;

    // Start is called before the first frame update
    private void Start()
    {
        monsterMovement = GetComponent<MonsterMovement>();

        monsterStateGoToSound = GetComponent<MonsterStateGoToSound>(); 
        monsterStateGoToSound.OnExitState += ExitStateGoToSound;

        monsterStateInvestigatePoint = GetComponent<MonsterStateInvestigate>();
        monsterStateInvestigatePoint.OnExitState += ExitStateInvestigatePoint;

        monsterStateWander = GetComponent<MonsterStateWander>();

        monsterStateWander.EnterState();
        currentState = eMonsterState.Wander;
    }

    private void Update()
    {
        if (currentState == eMonsterState.GoToSound || currentState == eMonsterState.SprintToSound)
        {
            monsterStateGoToSound.Tick();
        }
        else if (currentState == eMonsterState.Investigate)
        {
            monsterStateInvestigatePoint.Tick();
        }
        else if (currentState == eMonsterState.Wander)
        {
            monsterStateWander.Tick();
        }
    }

    private void ExitStateGoToSound()
    {
        currentState = eMonsterState.Investigate;
        monsterStateInvestigatePoint.EnterState(lastDetectedSound);
    }

    private void ExitStateInvestigatePoint()
    {
        currentState = eMonsterState.Wander;
        monsterStateWander.EnterState();
    }

    public void DetectSound(DetectedSound detectedSound)
    {
        // Detectable things send an event to us
        // TODO Check if we can hear it, and if its great priority than our lastDetectedSound
        lastDetectedSound = detectedSound;

        currentState = eMonsterState.GoToSound; // Sprint at some point
        monsterStateGoToSound.EnterState(lastDetectedSound, currentState);
    }

    // Returns a random valid point on the navmesh within radius of position
    // https://answers.unity.com/questions/475066/how-to-get-a-random-point-on-navmesh.html
    static public Vector3 GetRandomNavmeshPoint(Vector3 position, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, 1);

        return hit.position;
    }

    static public float InverseDistanceValue(Vector3 position1, Vector3 position2)
    {
        Vector3 distance = position1 - position2;

        return (1f / distance.magnitude);
    }
}
