using UnityEngine;

public class PlayerFootstepEmitter : Emitter
{
    
    public void PlayFootstep(float volume)
    {
        // TODO setting volume doesn't seem to work.
        soundMIDI.SetVolume(volume);
        PlaySound();
    }
}
