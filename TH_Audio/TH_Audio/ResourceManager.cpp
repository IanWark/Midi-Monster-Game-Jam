// HACKY RESOURCE MANAGEMENT!
#include "ResourceManager.h"

#include <vector>
#include <map>
#include <string>

#include "Audio/midi.h"

namespace
{
	int gNextID = 1;
	std::map<int, Audio::MIDI*> gLoadedMidi;
}

int LoadMidiResource(const char* path)
{
	int id = gNextID;
	gNextID++;

	Audio::MIDI* midi = new Audio::MIDI();
	midi->Load(path);
	gLoadedMidi[id] = midi;
	return id;
}

void UnloadAllResources()
{
	gNextID = 1;
	gLoadedMidi.clear();
}

Audio::MIDI* GetLoadedMIDI(int id)
{
	return gLoadedMidi[id];
}