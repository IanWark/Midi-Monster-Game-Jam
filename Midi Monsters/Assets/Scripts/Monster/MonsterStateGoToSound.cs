using System;

public class MonsterStateGoToSound
{
    public event Action OnExitState;
    
    Monster monster;
    MonsterMovement monsterMovement;

    public MonsterStateGoToSound(Monster monster, MonsterMovement monsterMovement)
    {
        this.monster = monster;
        this.monsterMovement = monsterMovement;
    }

    public void EnterState(Monster.DetectedSound detectedSound, Monster.eMonsterState monsterState)
    {
        // I think we do the distance check in here.
        monsterMovement.MoveToPosition(detectedSound.predictedPosition, monsterMovement.RunningSpeed);
    }

    public void Update()
    {
        if (monsterMovement.IsAtDestination())
        {
            OnExitState?.Invoke();
        }
    }
}
