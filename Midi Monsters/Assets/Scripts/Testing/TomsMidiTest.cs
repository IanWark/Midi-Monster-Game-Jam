using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomsMidiTest : MonoBehaviour
{
    private TH_Audio.MIDISound bgm;
    private TH_Audio.MIDISound bloop;

    // Start is called before the first frame update
    void Start()
    {
        bgm = new TH_Audio.MIDISound("Assets\\MIDI\\loop2.mid");
        bloop = new TH_Audio.MIDISound("Assets\\MIDI\\bloop.mid");

        bgm.Play(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            bloop.Play();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            bgm.SetBPM(100);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            bgm.SetBPM(300);
        }
    }
}
