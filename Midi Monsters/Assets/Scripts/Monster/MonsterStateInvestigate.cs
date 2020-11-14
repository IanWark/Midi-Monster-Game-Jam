using System;

public class MonsterStateInvestigatePoint
{
    public event Action OnExitState;

    Monster monster;
    MonsterMovement monsterMovement;

    public MonsterStateInvestigatePoint(Monster monster, MonsterMovement monsterMovement)
    {
        this.monster = monster;
        this.monsterMovement = monsterMovement;
    }

    public void EnterState(Monster.DetectedSound detectedSound)
    {
        // do nothing for now
    }

    public void Update()
    {
        // Just immediately skip this state
        OnExitState?.Invoke();
    }
}
