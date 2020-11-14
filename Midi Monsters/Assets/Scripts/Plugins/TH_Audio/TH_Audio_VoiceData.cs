using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AudioVoiceData")]
public class TH_Audio_VoiceData : ScriptableObject
{
    [SerializeField]
    public TH_Audio.VoiceParameters[] ChannelVoices = new TH_Audio.VoiceParameters[16];
}
