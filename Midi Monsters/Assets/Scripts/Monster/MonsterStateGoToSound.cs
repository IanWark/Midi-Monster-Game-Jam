using System;
using UnityEngine;

public class MonsterStateGoToSound : MonoBehaviour
{
    public event Action OnExitState;
    
    Monster monster;
    MonsterMovement monsterMovement;

    public void Start()
    {
        monster = GetComponent<Monster>();
        monsterMovement = GetComponent<MonsterMovement>();
    }

    public void EnterState(Monster.DetectedSound detectedSound, Monster.eMonsterState monsterState)
    {
        // I think we do the distance check in here.
        monsterMovement.MoveToPosition(detectedSound.predictedPosition, monsterMovement.RunningSpeed);
    }

    public void Tick()
    {
        if (monsterMovement.IsAtDestination())
        {
            OnExitState?.Invoke();
        }
    }
}
