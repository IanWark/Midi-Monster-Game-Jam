using System.Collections;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [SerializeField]
    private float spawnMonsterDelay = 5;

    [Header("Upgrade Monster Level 2")]
    [SerializeField, Range(0, 1), Tooltip("Threshold a sound must pass to be heard.")]
    private float lvl2HearSoundThreshold = 0.25f;
    [SerializeField, Range(0, 1.5f), Tooltip("Threshold a sound must pass for the monster to move at full speed.")]
    private float lvl2SprintThreshold = 0.75f;
    [SerializeField]
    private float lvl2WalkingSpeed = 2f;
    [SerializeField]
    private float lvl2RunningSpeed = 4f;
    [SerializeField]
    private float lvl2SprintingSpeed = 6f;
    [SerializeField, Tooltip("Max range from the player we will wander to.")]
    private float lvl2WanderPointFromPlayerRadius = 40f;

    [Header("Upgrade Monster Level 3")]
    [SerializeField, Range(0, 1), Tooltip("Threshold a sound must pass to be heard.")]
    private float lvl3HearSoundThreshold = 0.25f;
    [SerializeField, Range(0, 1.5f), Tooltip("Threshold a sound must pass for the monster to move at full speed.")]
    private float lvl3SprintThreshold = 0.75f;
    [SerializeField]
    private float lvl3WalkingSpeed = 2f;
    [SerializeField]
    private float lvl3RunningSpeed = 4f;
    [SerializeField]
    private float lvl3SprintingSpeed = 6f;
    [SerializeField, Tooltip("Max range from the player we will wander to.")]
    private float lvl3WanderPointFromPlayerRadius = 40f;

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
        else if (monsterLevel == 2)
        {
            UpgradeMonsterLevel2();
        }
        else if (monsterLevel == 3)
        {
            UpgradeMonsterLevel3();
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

    public void UpgradeMonsterLevel2()
    {
        monster.hearSoundThreshold = lvl2HearSoundThreshold;
        monster.sprintThreshold = lvl2SprintThreshold;
        monsterMovement.walkingSpeed = lvl2WalkingSpeed;
        monsterMovement.runningSpeed = lvl2RunningSpeed;
        monsterMovement.sprintingSpeed = lvl2SprintingSpeed;
        monsterStateWander.wanderPointFromPlayerRadius = lvl2WanderPointFromPlayerRadius;
    }

    public void UpgradeMonsterLevel3()
    {
        monster.hearSoundThreshold = lvl3HearSoundThreshold;
        monster.sprintThreshold = lvl3SprintThreshold;
        monsterMovement.walkingSpeed = lvl3WalkingSpeed;
        monsterMovement.runningSpeed = lvl3RunningSpeed;
        monsterMovement.sprintingSpeed = lvl3SprintingSpeed;
        monsterStateWander.wanderPointFromPlayerRadius = lvl3WanderPointFromPlayerRadius;
    }
}
