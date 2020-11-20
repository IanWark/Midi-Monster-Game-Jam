#pragma once

#include "Audio/voice.h"

#define DllExport extern "C" __declspec(dllexport)

DllExport void Init(void);
DllExport void Shutdown(void);
DllExport int LoadMidi(const char* path);
DllExport void PlayMidi(int id, bool loop);
DllExport void SetMIDITempo(int id, double bpm);
DllExport void SetMIDIVolume(int id, float volume);

DllExport void SetMasterVolume(float volume);

DllExport void SetChannelVoice(int channel, Audio::VoiceParameters params);
DllExport void SetChannelVolume(int channel, float volume);

bool CanQuitTest() { return false; }