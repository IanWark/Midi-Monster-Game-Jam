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
    [SerializeField, Tooltip("Max range from the player we will wander to.")]
    private List<float> wanderPointFromPlayerRadii = null;

    [Header("References")]
    [SerializeField]
    private Monster monster;
    [SerializeField]
    private MonsterMovement monsterMovement;
    [SerializeField]
    private MonsterStateWander monsterStateWander;

    private int monsterLevel = 0;

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
        monsterStateWander.wanderPointFromPlayerRadius = ValueFromList(monsterStateWander.wanderPointFromPlayerRadius, newLevel, wanderPointFromPlayerRadii);
    }

    public float ValueFromList(float defaultValue, int newLevel, List<float> list)
    {
        if (list != null && list.Count > newLevel)
        {
            return list[newLevel];
        }

        Debug.Assert(false, "MonsterManager - Cannot get value from list, using default value.");
        return defaultValue;
    }
}
