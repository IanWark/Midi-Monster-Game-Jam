using UnityEngine;

public class MonsterAudioManager : MonoBehaviour
{
    [Header("Monster State Values")]
    [SerializeField, Range(0,1), Tooltip("Value to return when in the chill state where we are randomly trying to find the player.")]
    private float wanderValue = 0f;
    [SerializeField, Range(0, 1), Tooltip("Value to return when in the state after we've gone to a sound's location and didn't find the player.")]
    private float investigateValue = 0.25f;
    [SerializeField, Range(0, 1), Tooltip("Value to return when we are running to a sound we heard.")]
    private float runValue = 0.75f;
    [SerializeField, Range(0, 1), Tooltip("Value to return when we are going max speed to a sound we heard.")]
    private float sprintValue = 1f;

    private Monster monster;

    void Start()
    {
        monster = GetComponent<Monster>();
    }

    public float GetMonsterState()
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
