#pragma once

namespace Audio
{
    class MIDI;
}

int LoadMidiResource(const char* path);
void UnloadAllResources();

 Audio::MIDI* GetLoadedMIDI(int id);