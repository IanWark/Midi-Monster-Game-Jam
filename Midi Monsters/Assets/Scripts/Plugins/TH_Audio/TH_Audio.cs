using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;

public class TH_Audio : MonoBehaviour
{
    [System.Serializable]
    public enum WaveShape : int
    {
        OFF = 0,
		SINE = 1,
		SQUARE = 2,
		TRIANGLE = 3	
	};

    [System.Serializable]
    public struct OscillatorParameters
    {
        public WaveShape Shape;
    };

    [System.Serializable]
    public struct ADSRParameters
    {
        public double Attack;
        public double Decay;
        public double Sustain;
        public double Release;
    };

    [System.Serializable]
    public struct VoiceParameters
    {
        public OscillatorParameters Osc1Params;
        public OscillatorParameters Osc2Params;
        public ADSRParameters ADSRParams;
        public int Function;
        public double Volume;
    };

    [DllImport("TH_Audio.dll")]
    private static extern void Init();

    [DllImport("TH_Audio.dll")]
    private static extern void Shutdown();

    [DllImport("TH_Audio.dll")]
    private static extern int LoadMidi(string path);

    [DllImport("TH_Audio.dll")]
    private static extern void PlayMidi(int id, bool loop);

    [DllImport("TH_Audio.dll")]
    private static extern void SetChannelVoice(int channel, VoiceParameters parameters);

    [DllImport("TH_Audio.dll")]
    private static extern void SetChannelVolume(int channel, float volume);

    [DllImport("TH_Audio.dll")]
    private static extern void SetMIDITempo(int id, double bpm);

    [SerializeField]
    private TH_Audio_VoiceData voiceData;

    void Awake()
    {
        TH_Audio.Init();

        for (int i = 0; i < 16; i++)
        {
            SetChannelVoice(i, voiceData.ChannelVoices[i]);
        }
    }

    void OnDestroy()
    {
        TH_Audio.Shutdown();
    }

    public class MIDISound
    {
        private int handle = -1;
        public MIDISound(string path)
        {
            Debug.Assert(!string.IsNullOrEmpty(path), "Midi file path cannot be empty.");
            handle = LoadMidi(path);
        }

        public void Play(bool looping = false)
        {
            PlayMidi(handle, looping);
        }

        public void SetBPM(double bpm)
        {
            SetMIDITempo(handle, bpm);
        }
    }

}
