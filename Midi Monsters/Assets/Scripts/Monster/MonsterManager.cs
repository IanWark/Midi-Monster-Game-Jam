using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [SerializeField]
    private float spawnMonsterDelay = 5;

    [SerializeField, Tooltip("Threshold a sound must pass to be heard, 0 to 1.")]
    private List<float> hearSoundThresholds = null;
    [SerializeField, Tooltip("Threshold a sound must pass for the monster to move at full speed, 0 to 1.")]
    private List<float> sprintThresholds = null;
    [SerializeField, Tooltip("Speed we move when walking.")]
    private List<float> walkingSpeeds = null;
    [SerializeField, Tooltip("Normal speed we move when going to a sound.")]
    private List<float> runningSpeeds = null;
    [SerializeField, Tooltip("Full speed we move when going to a loud sound.")]
    private List<float> sprintingSpeeds = null;
    [SerializeField, Tooltip("Max range from the wanderPointCenter we will wander to.")]
    private List<float> wanderPointRadii = null;
    [SerializeField, Tooltip("Point that we wander around. Can be the player.")]
    private List<Transform> wanderPointCenters = null;
    [SerializeField, Tooltip("How far away from the last sound we heard we will search.")]
    private List<float> investigateRadii = null;

    [Header("References")]
    [SerializeField]
    private Monster monster = null;
    [SerializeField]
    private MonsterMovement monsterMovement = null;
    [SerializeField]
    private MonsterStateWander monsterStateWander = null;
    [SerializeField]
    private MonsterStateInvestigate monsterStateInvestigate = null;

    [SerializeField]
    private int monsterLevel = -1;

    private void Start()
    {
        LevelUpMonster();
    }

    public void LevelUpMonster()
    {
        monsterLevel++;

        if (monsterLevel == 1)
        {
            SpawnMonster();
        }

        if (monsterLevel >= 1)
        {
            UpgradeMonster(monsterLevel - 1);
        }
    }

    public void SpawnMonster()
    {
        StartCoroutine(SpawningCoroutine());
    }

    private IEnumerator SpawningCoroutine()
    {
        yield return new WaitForSeconds(spawnMonsterDelay);

        monster.gameObject.SetActive(true);
    }

    public void UpgradeMonster(int newLevel)
    {
        monster.hearSoundThreshold = ValueFromList(monster.hearSoundThreshold, newLevel, hearSoundThresholds);
        monster.sprintThreshold = ValueFromList(monster.sprintThreshold, newLevel, sprintThresholds);
        monsterMovement.walkingSpeed = ValueFromList(monsterMovement.walkingSpeed, newLevel, walkingSpeeds);
        monsterMovement.runningSpeed = ValueFromList(monsterMovement.runningSpeed, newLevel, runningSpeeds);
        monsterMovement.sprintingSpeed = ValueFromList(monsterMovement.sprintingSpeed, newLevel, sprintingSpeeds);
        monsterStateWander.wanderPointRadius = ValueFromList(monsterStateWander.wanderPointRadius, newLevel, wanderPointRadii);
        monsterStateWander.wanderPointCenter = ValueFromList(monsterStateWander.wanderPointCenter, newLevel, wanderPointCenters);
        monsterStateInvestigate.investigateRadius = ValueFromList(monsterStateInvestigate.investigateRadius, newLevel, investigateRadii);
    }

    // Get list[newLevel]. If we cannot, return defaultValue
    public T ValueFromList<T>(T defaultValue, int newLevel, List<T> list)
    {
        if (list != null && list.Count > newLevel)
        {
            return list[newLevel];
        }

        Debug.Assert(false, "MonsterManager - Cannot get value from list, using default value.");
        return defaultValue;
    }
}
