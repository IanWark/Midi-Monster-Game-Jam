using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;

public class TH_Audio : MonoBehaviour
{
    [DllImport("TH_Audio.dll")]
    private static extern void Init();

    [DllImport("TH_Audio.dll")]
    private static extern void Shutdown();

    [DllImport("TH_Audio.dll")]
    private static extern int LoadMidi(string path);

    [DllImport("TH_Audio.dll")]
    private static extern void PlayMidi(int id, bool loop);

    void Awake()
    {
        TH_Audio.Init();
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
    }

}
