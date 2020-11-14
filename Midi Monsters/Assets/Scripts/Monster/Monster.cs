using UnityEngine;

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
        GoToSound = 1,
        Investigate = 2,
        Wander = 3,
    }

    [SerializeField]
    private Transform startWaypoint;
    
    private eMonsterState currentState = eMonsterState.Wander;
    private DetectedSound lastDetectedSound = null;

    MonsterStateGoToSound monsterStateGoToSound;
    MonsterStateInvestigatePoint monsterStateInvestigatePoint;
    MonsterStateWander monsterStateWander;

    MonsterMovement monsterMovement;

    // Start is called before the first frame update
    private void Start()
    {
        monsterMovement = GetComponent<MonsterMovement>();

        monsterStateGoToSound = new MonsterStateGoToSound(this, monsterMovement);
        monsterStateGoToSound.OnExitState += ExitStateGoToSound;

        monsterStateInvestigatePoint = new MonsterStateInvestigatePoint(this, monsterMovement);
        monsterStateInvestigatePoint.OnExitState += ExitStateInvestigatePoint;

        monsterStateWander = new MonsterStateWander(this, monsterMovement, startWaypoint);

        monsterStateWander.EnterState();
        currentState = eMonsterState.Wander;
    }

    private void Update()
    {
        if (currentState == eMonsterState.GoToSound)
        {
            monsterStateGoToSound.Update();
        }
        else if (currentState == eMonsterState.Investigate)
        {
            monsterStateInvestigatePoint.Update();
        }
        else if (currentState == eMonsterState.Wander)
        {
            monsterStateWander.Update();
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

        currentState = eMonsterState.GoToSound;
        monsterStateGoToSound.EnterState(lastDetectedSound);
    }


}
