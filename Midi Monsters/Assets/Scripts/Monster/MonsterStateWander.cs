using System;
using UnityEngine;

public class MonsterStateWander
{
    Monster monster;
    MonsterMovement monsterMovement;
    Transform startWaypoint;

    public MonsterStateWander(Monster monster, MonsterMovement monsterMovement, Transform startWaypoint)
    {
        this.monster = monster;
        this.monsterMovement = monsterMovement;
        this.startWaypoint = startWaypoint;
    }

    public void EnterState()
    {
        // TODO for now, we move back to start waypoint
        monsterMovement.MoveToPosition(startWaypoint.position, monsterMovement.WalkingSpeed);
    }

    public void Update()
    {
        
    }
}
