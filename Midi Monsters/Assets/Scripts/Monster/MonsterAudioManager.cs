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

    [Header("Modifiers")]
    [SerializeField, Tooltip("The angle where the playerSeesMonster value is set to 0. 180 is completely away from the monster.")]
    private float playerSeesMonsterMaxAngle = 180;

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
        return Mathf.Clamp(dotProduct, 0, 1); 
    }

    public float GetMonsterProximityValue()
    {
        // TODO add designer modifiable modifiers
        return Monster.InverseDistanceValue(player.transform.position, monster.transform.position);
    }

    public float GetMonsterSeesPlayerValue()
    {
        switch(monster.CurrentState)
        {
            case Monster.eMonsterState.Wander:
                return wanderValue;
            case Monster.eMonsterState.Investigate:
                return investigateValue;
            case Monster.eMonsterState.GoToSound:
                return runValue;
            case Monster.eMonsterState.SprintToSound:
                return sprintValue;
        }

        // Oh no!
        Debug.Assert(false, "GetMonsterState - Monster in invalid state!");
        return wanderValue;
    }
}
