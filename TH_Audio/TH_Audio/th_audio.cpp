#include "th_audio.h"

#include "Audio/audioManager.h"
#include "Audio/midi.h"
#include "ResourceManager.h"

#include <iostream>

void Init(void)
{
	std::cout << "TH_AUDIO INIT";
	Audio::GameAudioManager::Init();
}

void Shutdown(void)
{
	UnloadAllResources();
	Audio::GameAudioManager::Uninit();
}

int LoadMidi(const char* path)
{
	return LoadMidiResource(path);
}

void PlayMidi(int id, bool loop)
{
	Audio::MIDI* midi = GetLoadedMIDI(id);
	Audio::GameAudioManager::Play(midi, loop); // Is this thread safe?  Probably not
}

void SetMIDITempo(int id, double bpm)
{
	Audio::MIDI* midi = GetLoadedMIDI(id);
	midi->SetBPM(bpm);
}

void SetMIDIVolume(int id, float volume)
{
	Audio::MIDI* midi = GetLoadedMIDI(id);
	midi->SetVolume(volume);
}

void SetChannelVoice(int channel, Audio::VoiceParameters params)
{
	Audio::GameAudioManager::SetChannelVoice(channel, params);
}

void SetChannelVolume(int channel, float volume)
{
	Audio::GameAudioManager::SetChannelVolume(channel, volume);
}