using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter : MonoBehaviour
{
    [SerializeField]
    private GameAudioInterface.MIDISfx Sound;

    public TH_Audio.MIDISound soundMIDI;

    public bool AutoPlay = false;
    public bool Loop = false;

    private void Start()
    {
        soundMIDI = new TH_Audio.MIDISound("Assets\\MIDI\\SFX\\" + Sound.ToString() + ".mid");
        GameAudioInterface.RegisterEmitter(this);
        if (AutoPlay)
        {
            PlaySound();
        }
    }

    public void PlaySound()
    {
        soundMIDI.Play(Loop);
    }
}
