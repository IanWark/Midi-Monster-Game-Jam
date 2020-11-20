using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioInterface : MonoBehaviour
{
    private TH_Audio.MIDISound layeredMusic;

    public static GameAudioInterface Instance; // Lol

    [SerializeField]
    private PlayerCharacterController player;

    public enum MIDISfx
    {
        Cymbal,
        Piano,
        AlarmClock,
        CoffeePot,
        PlayerDeath,
        MonsterFootstep,
        PlayerFootstep,
        Piano2,
        GuitarStrum,
        BassMoney,
        WUB,
        Roar,
        COUNT
    }

    public struct MonsterAudioUpdate
    {
        public float PlayerSeesMonster;
        public float MonsterSeesPlayer;
        public float Proximity;
    }

    private Dictionary<MIDISfx, TH_Audio.MIDISound> soundLibrary = new Dictionary<MIDISfx, TH_Audio.MIDISound>();

    private List<Emitter> runningEmitters = new List<Emitter>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        layeredMusic = new TH_Audio.MIDISound("Assets\\MIDI\\LayeredBGM.mid");
        layeredMusic.Play(true);

        for (MIDISfx i = 0; i < MIDISfx.COUNT; i++)
        {
            soundLibrary[i] = new TH_Audio.MIDISound("Assets\\MIDI\\SFX\\" + i.ToString() + ".mid");
        }

        TH_Audio.SetChannelVolume(0, 0);
        TH_Audio.SetChannelVolume(1, 0);
        TH_Audio.SetChannelVolume(2, 0);
        TH_Audio.SetChannelVolume(3, 0);
        TH_Audio.SetChannelVolume(9, 0);

        TH_Audio.SetMasterVolume(1);
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

    public void PlayMIDISfx(MIDISfx effect)
    {
        soundLibrary[effect].Play();
    }

    private void Update()
    {
        if (player)
        {
            foreach (Emitter emitter in runningEmitters)
            {
                float distance = Vector3.Distance(player.transform.position, emitter.transform.position);
                distance =  Mathf.Clamp01((20.0f - distance)/20.0f); // Lol hack
                emitter.soundMIDI.SetVolume(distance);
            }
        }        
    }

    public static void RegisterEmitter(Emitter emitter)
    {
        Instance.runningEmitters.Add(emitter);
    }
}
