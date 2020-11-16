using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public class DetectedSound
    {
        public Vector3 position;
        public Vector3 predictedPosition;
        public float volume;

        public DetectedSound(Vector3 position, Vector3 predictedPosition, float volume)
        {
            this.position = position;
            this.predictedPosition = predictedPosition;
            this.volume = volume;
        }

        public float GetPriority(Vector3 monsterPosition)
        {
            return InverseDistanceValue(monsterPosition, position) * volume;
        }
    }

    public enum MonsterState
    {
        SprintToSound = 0,
        GoToSound = 1,
        Investigate = 2,
        Wander = 3,
    }

    [SerializeField]
    private bool debugPrint = false;

    [SerializeField, Range(0, 1), Tooltip("Threshold a sound must pass to be heard.")]
    public float hearSoundThreshold = 0.25f;

    [SerializeField, Range(0, 1.5f), Tooltip("Threshold a sound must pass for the monster to move at full speed.")]
    public float sprintThreshold = 0.75f;

    private MonsterState currentState = MonsterState.Wander;
    public MonsterState CurrentState { get { return currentState; } }
    private DetectedSound lastDetectedSound = null;
    public float timeSinceLastSound { get; private set; }

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
        currentState = MonsterState.Wander;
    }

    private void Update()
    {
        if (debugPrint)
        {
            Debug.Log("currentState: " + currentState);
        }

        timeSinceLastSound += Time.deltaTime;

        if (currentState == MonsterState.GoToSound || currentState == MonsterState.SprintToSound)
        {
            monsterStateGoToSound.Tick();
        }
        else if (currentState == MonsterState.Investigate)
        {
            monsterStateInvestigatePoint.Tick();
        }
        else if (currentState == MonsterState.Wander)
        {
            monsterStateWander.Tick();
        }
    }

    private void ExitStateGoToSound()
    {
        currentState = MonsterState.Investigate;
        monsterStateInvestigatePoint.EnterState(lastDetectedSound);
    }

    private void ExitStateInvestigatePoint()
    {
        lastDetectedSound = null;
        currentState = MonsterState.Wander;
        monsterStateWander.EnterState();
    }

    // When a sound make a noise, it calls this to alert the monster to it
    public void DetectSound(DetectedSound detectedSound)
    {
        if (monsterStateGoToSound != null)
        {
            // Sound priority must be above threshold
            float newSoundPriority = detectedSound.GetPriority(transform.position);
            if (newSoundPriority > hearSoundThreshold)
            {
                lastDetectedSound = detectedSound;
                timeSinceLastSound = 0;

                currentState = newSoundPriority > sprintThreshold ? MonsterState.SprintToSound : MonsterState.GoToSound;
                monsterStateGoToSound.EnterState(lastDetectedSound, currentState);
            }
        }
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
        return (1f / Vector3.Distance(position1, position2));
    }
}
