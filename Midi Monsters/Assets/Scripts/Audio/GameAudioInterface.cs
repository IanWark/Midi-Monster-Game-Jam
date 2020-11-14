using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioInterface : MonoBehaviour
{
    private TH_Audio.MIDISound layeredMusic;

    [SerializeField]
    private PlayerCharacterController player;

    public enum MIDISfx
    {
        Cymbal,
        Piano,
        AlarmClock,
        CoffeePot,
        PlayerDeath,
        COUNT
    }

    public struct MonsterAudioUpdate
    {
        public float PlayerSeesMonster;
        public float MonsterSeesPlayer;
        public float Proximity;
    }

    private Dictionary<MIDISfx, TH_Audio.MIDISound> soundLibrary = new Dictionary<MIDISfx, TH_Audio.MIDISound>();

    private void Start()
    {
        layeredMusic = new TH_Audio.MIDISound("Assets\\MIDI\\LayeredBGM.mid");
        layeredMusic.Play(true);

        for (MIDISfx i = 0; i < MIDISfx.COUNT; i++)
        {
            soundLibrary[i] = new TH_Audio.MIDISound("Assets\\MIDI\\SFX\\" + i.ToString() + ".mid");
        }
    }

    public void UpdateMonsterState(MonsterAudioUpdate update)
    {
        TH_Audio.SetChannelVolume(0, 0.05f);
        TH_Audio.SetChannelVolume(1, 0.05f * update.PlayerSeesMonster);
        TH_Audio.SetChannelVolume(2, 0.05f * update.MonsterSeesPlayer);
        TH_Audio.SetChannelVolume(3, 0.03f * update.Proximity);
        TH_Audio.SetChannelVolume(9, 0.2f * update.Proximity);

        layeredMusic.SetBPM(140 + (update.Proximity * 600));
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
