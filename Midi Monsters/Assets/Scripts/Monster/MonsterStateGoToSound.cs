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

    public void EnterState(Monster.DetectedSound detectedSound, Monster.MonsterState monsterState)
    {
        float movementSpeed = monsterMovement.RunningSpeed;
        if (monsterState == Monster.MonsterState.SprintToSound)
        {
            movementSpeed = monsterMovement.SprintingSpeed;
        }
        
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
