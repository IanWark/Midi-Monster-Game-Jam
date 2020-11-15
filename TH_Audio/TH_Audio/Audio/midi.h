#ifndef MIDI_H
#define MIDI_H

//#include "utils/resourceManager.h"

#include <mutex>
#include <vector>

namespace Tasks
{
	class MIDILoadTask;
	struct MIDILoadTaskParams;
}

namespace Audio
{
	enum MIDIEventType : unsigned char
	{
		NOTE_OFF = 0x80,
		NOTE_ON = 0x90,
		KEY_PRESSURE = 0xA0,
		CONTROLLER_CHANGE = 0xB0,
		PROGRAM_CHANGE = 0xC0,
		CHANNEL_KEY_PRESSURE = 0xD0,
		PITCH_BEND = 0xE0
	};

	struct MIDIEvent
	{
		int Time;		
		MIDIEventType Type;
		int Channel;
		int Track;
		unsigned char Data1;
		unsigned char Data2;
	};

	class MIDI// : public Utils::IResource
	{
	private:
		bool mIsAsync;
		bool mIsReady;

		bool mLoop;

		double mPulsePerQuarterNote;
		double mUSecondPerQuarterNote;
		double mBPM;
		double mTicksPerSecond;

		float mInstanceVolume;

		std::mutex mMutex;
		Tasks::MIDILoadTaskParams* mTaskParams;

		std::string mPath;
		
		std::vector<MIDIEvent> mMIDIEventData;

		bool LoadInternal(const char* path);
		bool UnloadInternal();
		bool ReloadInternal();

		int ReadEvent(FILE* file, int trackIndex);
		int ReadMIDIEvent(FILE* file, unsigned char type, int trackIndex);
		int ReadSysexEvent(FILE* file, int trackIndex);
		int ReadMetaEvent(FILE* file, int trackIndex);

	public:
		MIDI();
		virtual ~MIDI();

		// From Utils::IResource
		virtual void Load(const char*);
		virtual void LoadAsync(const char*);
		virtual void Unload();
		virtual void UnloadAsync();
		virtual void Reload();
		virtual void ReloadAsync();
		virtual bool IsLoaded();

		void SetLoop(bool value) { mLoop = value; }
		bool GetLoop() const { return mLoop; }

		const double GetTickRate() const { return mTicksPerSecond; }

		double GetBPM() const { return mBPM; }
		float GetInstanceVolume() const { return mInstanceVolume; }

		void SetBPM(double bpm);
		void SetVolume(float volume);

		const std::vector<MIDIEvent>& GetEvents() const { return mMIDIEventData; }

		//friend class Tasks::MIDILoadTask;
		//friend class Utils::ResourceManager;
	};


}

#endif

