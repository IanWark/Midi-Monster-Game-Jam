using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter : MonoBehaviour
{
    [SerializeField]
    private GameAudioInterface.MIDISfx Sound;

    public float Volume { get; set; }

    public void PlaySound()
    {
        // Todo
    }
}
