using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioInterface : MonoBehaviour
{
    public enum MIDISfx
    {
        Bloop
    }

    private Dictionary<MIDISfx, TH_Audio.MIDISound> soundLibrary = new Dictionary<MIDISfx, TH_Audio.MIDISound>();

    public void UpdateMonsterState(/*Monster State Struct Here*/)
    {
        Debug.Log("Monster State Updated");
    }

    public void PlayNote(float freq, int channel, float length)
    {
        Debug.Log("Playing Note");
    }

    public void PlayMIDISfx(MIDISfx effect)
    {
        Debug.Log("Playing SFX");
    }
}
