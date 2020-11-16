using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFootstepEmitter : Emitter
{
    [SerializeField]
    MonsterMovement movement;

    [SerializeField]
    private float timeBetweenSteps = 1;

    public ParticleSystem particles;

    public float StepSpeedScaler = 30.0f;

    private float stepTimer = 1;

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

            // Increase rate based on how fast we are moving compared to slowest speed
            stepTimer += (UnityEngine.Time.deltaTime * (movement.GetAgentSpeed() / movement.walkingSpeed));
            if (stepTimer > (timeBetweenSteps))
            {
                stepTimer = 0;

                particles.Play();
                PlaySound();
            }

        }
    }
}
