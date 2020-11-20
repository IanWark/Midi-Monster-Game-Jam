#ifndef AUDIO_MANAGER_H
#define AUDIO_MANAGER_H

#include <thread>
#include <mutex>
#include <list>
#include <vector>

#include "voice.h"

struct IAudioClient;
struct IAudioRenderClient;
struct ISimpleAudioVolume;
struct tWAVEFORMATEX;
typedef tWAVEFORMATEX WAVEFORMATEX;

namespace Audio
{
	class Sound;
	class MIDI;
	class Voice;
	struct MIDIEvent;

	struct PlayingSoundInstance
	{
		Sound* Sound;
		unsigned int Position;

		long long Id;

		bool operator==(const PlayingSoundInstance& that) const
		{
			return (Id == that.Id);
		}
	};

	struct PlayingMIDIInstance
	{
		MIDI* Sound;
		double Position;
		unsigned int EventIndex;

		long long Id;
		bool Looping;

		void Start();
		void Step(const double& dt);

		bool operator==(const PlayingMIDIInstance& that)
		{
			return (Id == that.Id);
		}
	};

	class GameAudioManager
	{
	private: 

		std::mutex mMutex;

		IAudioClient* mAudioClient;
		IAudioRenderClient* mRenderClient;
		ISimpleAudioVolume* mVolumeService;
		WAVEFORMATEX* mWaveFormat;

		static std::thread* mThread;

		bool mMute;

		float mMasterVolume;

		std::vector<Voice*> mVoices;

		std::list<PlayingSoundInstance> mPlayingSounds;
		std::list<Sound*> mSoundQueue;

		std::list<PlayingMIDIInstance> mPlayingMIDI;
		std::list<MIDI*> mMIDIQueue;

		void CheckResult(const char* operation, long result);

		void LogFormat(WAVEFORMATEX*);

		GameAudioManager();
		~GameAudioManager();

		void FillPlaceholderAudioData(unsigned char* data, int size);

		static void StartThread();

		void InitInternal();
		void UninitInternal();		

		unsigned int GetSampleRateInternal() const;
		void PlayInternal(Sound* sound);
		void PlayInternal(MIDI* sound, bool loop);

		void HandleMIDIEventInternal(MIDIEvent evt, float instanceVolume);

		void SetChannelVoiceInternal(int channel, VoiceParameters params);
		void SetChannelVolumeInternal(int channel, float volume);
		void SetMasterVolumeInternal(float volume);

	public:
		static void Init();
		static void Uninit();

		static void NoteOn(unsigned char note);
		static void NoteOff(unsigned char note);

		static unsigned int GetSampleRate();
		static void Play(Sound* sound);
		static void Play(MIDI* sound, bool loop);

		static void Test();

		static void HandleMIDIEvent(MIDIEvent evt, float instanceVolume);

		static int SecondsToSamples(double time);

		static void SetChannelVoice(int channel, VoiceParameters params);
		static void SetChannelVolume(int channel, float volume);

		static void SetMasterVolume(float volume);

		void Update();
	};
}

#endif