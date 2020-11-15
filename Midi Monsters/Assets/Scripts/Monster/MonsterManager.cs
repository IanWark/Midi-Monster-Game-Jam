using System.Collections;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [SerializeField]
    private Monster monster;

    [SerializeField]
    private float spawnMonsterDelay = 5;

    public void SpawnMonster()
    {
        StartCoroutine(SpawningCoroutine());
    }

    private IEnumerator SpawningCoroutine()
    {
        yield return new WaitForSeconds(spawnMonsterDelay);

        monster.gameObject.SetActive(true);
    }
}
