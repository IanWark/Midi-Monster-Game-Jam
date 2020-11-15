#include "th_audio.h"

int main()
{
	Init();	

	int id = LoadMidi("loop1.mid");
	PlayMidi(id, true);

	Audio::VoiceParameters params;
	params.ADSRParams.Attack = 0.01;
	params.ADSRParams.Decay = 0.1;
	params.ADSRParams.Sustain = 0;
	params.ADSRParams.Release = 0;
	params.Osc1Params.Shape = Audio::WaveShape::SQUARE;
	params.Osc2Params.Shape = Audio::WaveShape::SQUARE;
	params.Function = Audio::ENV_OUTPUT | Audio::ENV_NOTE;
	params.Volume = 0.3;

	SetChannelVoice(9, params);

	SetMIDIVolume(id, 0.1);

	SetMIDITempo(id, 500);

	while (!CanQuitTest()) {}

	return 0;
}