using UnityEngine;

public class MonsterAudioManager : MonoBehaviour
{
    [SerializeField]
    private bool debugPrint = false;

    [Header("Monster State Values")]
    [SerializeField, Range(0,1), Tooltip("Value to return when in the chill state where we are randomly trying to find the player.")]
    private float wanderValue = 0f;
    [SerializeField, Range(0, 1), Tooltip("Value to return when in the state after we've gone to a sound's location and didn't find the player.")]
    private float investigateValue = 0.25f;
    [SerializeField, Range(0, 1), Tooltip("Value to return when we are running to a sound we heard.")]
    private float runValue = 0.75f;
    [SerializeField, Range(0, 1), Tooltip("Value to return when we are going max speed to a sound we heard.")]
    private float sprintValue = 1f;
    [SerializeField, Tooltip("Seconds before a sound is considered 'old'.")]
    private float oldSoundSeconds = 3f;
    [SerializeField, Range(0, 1), Tooltip("Multiplier for when we are running to a sound we heard a while ago. Only applies to run/sprint values.")]
    private float oldSoundValueMultiplier = 0.75f;

    [Header("Modifiers")]
    [SerializeField, Tooltip("The angle where the playerSeesMonster value is set to 0. 180 is completely away from the monster.")]
    private float playerSeesMonsterMaxAngle = 180;
    [SerializeField, Tooltip("Where we mulitply playerSeesMonster value by the proximity to the monster")]
    private bool playerSeesMonsterUseProximity = true;
    [SerializeField, Tooltip("Just a straight multiplier on the playerSeesMonster value")]
    private float playerSeesMonsterMultiplier = 2f;

    [Header("References")]
    [SerializeField]
    private PlayerCharacterController player;
    [SerializeField]
    private GameAudioInterface audioInterface;

    private Monster monster;

    void Start()
    {
        monster = GetComponent<Monster>();
    }

    private void Update()
    {
        // Update the audioInterface with our info
        if (audioInterface != null)
        {
            GameAudioInterface.MonsterAudioUpdate monsterAudioUpdate = new GameAudioInterface.MonsterAudioUpdate();
            monsterAudioUpdate.PlayerSeesMonster = GetPlayerSeesMonsterValue();
            monsterAudioUpdate.MonsterSeesPlayer = GetMonsterSeesPlayerValue();
            monsterAudioUpdate.Proximity = GetMonsterProximityValue();

            if (debugPrint)
            {
                Debug.Log("MonsterAudioUpdate: PlayerSeesMonster: " + monsterAudioUpdate.PlayerSeesMonster + " MonsterSeesPlayer: " + monsterAudioUpdate.MonsterSeesPlayer + " Proximity: " + monsterAudioUpdate.Proximity);
            }

            audioInterface.UpdateMonsterState(monsterAudioUpdate);
        } 
    }

    public float GetPlayerSeesMonsterValue()
    {
        // get dotproduct from player facing angle to monster's position
        Vector3 directionFromPlayerToMonster = (monster.transform.position - player.transform.position).normalized;
        float dotProduct = Vector3.Dot(player.transform.forward, directionFromPlayerToMonster);

        // make it only go away at the correct angle
        float angleValue = (playerSeesMonsterMaxAngle - 90f) / 180f;
        dotProduct = angleValue + (dotProduct * (1f - angleValue));

        // no negatives allowed, clamp it
        dotProduct = Mathf.Clamp(dotProduct, 0, 1);

        if (playerSeesMonsterUseProximity)
        {
            dotProduct *= GetMonsterProximityValue();
        }

        return dotProduct * playerSeesMonsterMultiplier; 
    }

    public float GetMonsterProximityValue()
    {
        // TODO add designer modifiable modifiers
        return Monster.InverseDistanceValue(player.transform.position, monster.transform.position);
    }

    public float GetMonsterSeesPlayerValue()
    {
        float value = wanderValue;

        switch (monster.CurrentState)
        {
            case Monster.MonsterState.Wander:
                value = wanderValue;
                break;
            case Monster.MonsterState.Investigate:
                value = investigateValue;
                break;
            case Monster.MonsterState.GoToSound:
                value = runValue;
                break;
            case Monster.MonsterState.SprintToSound:
                value = sprintValue;
                break;
            default:
                // Oh no!
                Debug.Assert(false, "GetMonsterState - Monster in invalid state!");
                break;
        }

        if (monster.timeSinceLastSound > oldSoundSeconds 
            && (monster.CurrentState == Monster.MonsterState.GoToSound || monster.CurrentState == Monster.MonsterState.SprintToSound))
        {
            value *= oldSoundValueMultiplier;
        }
       
        return value;
    }
}
