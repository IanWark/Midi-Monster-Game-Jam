using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFootstepEmitter : Emitter
{
    [SerializeField]
    MonsterMovement movement;

    public float StepSpeedScaler = 30.0f;

    // Update is called once per frame
    void Update()
    {        
        if (movement.IsAtDestination())
        {
            TH_Audio.SetChannelVolume(11, 0);
            soundMIDI.SetBPM(180);
        }
        else
        {
            TH_Audio.SetChannelVolume(11, 0.2f);
            soundMIDI.SetBPM(movement.GetAgentSpeed() * StepSpeedScaler);
        }
    }
}
