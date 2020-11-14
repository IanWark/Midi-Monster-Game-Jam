using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomsMidiTest : MonoBehaviour
{
    public GameAudioInterface audioInterface;

    [Range(0,1)]
    public float player = 1;

    [Range(0, 1)]
    public float monster = 1;

    [Range(0, 1)]
    public float prox = 1;

    private TH_Audio.MIDISound bloop;

    // Start is called before the first frame update
    void Start()
    {
        bloop = new TH_Audio.MIDISound("Assets\\MIDI\\bloop.mid");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            bloop.Play();
        }

        audioInterface.UpdateMonsterState(new GameAudioInterface.MonsterAudioUpdate() { MonsterSeesPlayer = monster, PlayerSeesMonster = player, Proximity = prox });
    }
}
