using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFootstepEmitter : Emitter
{
    [SerializeField]
    MonsterMovement movement;

    public ParticleSystem particles;

    public float StepSpeedScaler = 30.0f;

    public float timeUntilNextStep = 0;

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
            float stepsPerMinute = movement.GetAgentSpeed() * StepSpeedScaler;

            TH_Audio.SetChannelVolume(11, 0.2f);
            soundMIDI.SetBPM(stepsPerMinute);

            timeUntilNextStep -= UnityEngine.Time.deltaTime;
            if (timeUntilNextStep <= 0)
            {
                timeUntilNextStep = 1;

                particles.Play();
                PlaySound();
            }

        }
    }
}
