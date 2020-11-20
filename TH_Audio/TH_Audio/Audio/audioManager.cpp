#include "audioManager.h"

#include "../GameUtil/debugConsole.h"

#include <Windows.h>
#include <mmdeviceapi.h>
#include <Audioclient.h>
#include <audiopolicy.h>
#include <AudioSessionTypes.h>
#include <ksmedia.h>

#include <ctime>
#include <algorithm>

#include "../Memory/memory.h"

#include "voice.h"
#include "oscillator.h"
#include "effects/effectChain.h"
#include "effects/lowPass.h"
#include "effects/delay.h"

#include "sound.h"
#include "midi.h"

#include <iostream>
#include <atomic>

namespace Audio
{
	// Todo: Clean up all this nonsense:

	const unsigned int VOICE_COUNT = 30;	
	const unsigned int MIDI_CHANNEL_COUNT = 16;

	GameAudioManager* instance = nullptr;
	std::thread* GameAudioManager::mThread = nullptr;
	std::atomic<bool> audioRunning = false;

	Sound* TestSound = nullptr;
	MIDI* TestMusic = nullptr;

	EffectChain* Chain = nullptr;
		
	HANDLE audioEvent;

	VoiceParameters channelVoices[MIDI_CHANNEL_COUNT];

	//VoiceParameters defaultVoiceParams;
	//VoiceParameters percussionVoiceParams;

	void PlayingMIDIInstance::Start()
	{
		EventIndex = 0;
		Position = 0;
	}

	void PlayingMIDIInstance::Step(const double& dt) 
	{
		Position += dt * Sound->GetTickRate();
		const std::vector<MIDIEvent>& events = Sound->GetEvents();

		int currentSample = (int)Position;

		while (EventIndex < events.size() && events[EventIndex].Time < currentSample)
		{
			GameAudioManager::HandleMIDIEvent(events[EventIndex], Sound->GetInstanceVolume());
			EventIndex++;
			if (EventIndex == events.size() && Looping)
			{
				EventIndex = 0;
				Position = 0;
				currentSample = 0;
			}
		}
	}

	GameAudioManager::GameAudioManager()
		: mMutex()
		, mAudioClient(nullptr)
		, mRenderClient(nullptr)
		, mVolumeService(nullptr)
		, mWaveFormat(nullptr)
		, mMute(false)
		, mVoices()
		, mPlayingSounds()
		, mSoundQueue()
		, mPlayingMIDI()
		, mMIDIQueue()
		, mMasterVolume(0)
	{
		for (int i = 0; i < VOICE_COUNT; i++)
		{
			mVoices.push_back(CREATE(Voice));			
		}

		Voice::InitFrequencyMap();

		for (int i = 0; i < MIDI_CHANNEL_COUNT; i++)
		{
			channelVoices[i].ADSRParams.Attack = 0.001;
			channelVoices[i].ADSRParams.Decay = 0.5;
			channelVoices[i].ADSRParams.Sustain = 0;
			channelVoices[i].ADSRParams.Release = 0.2;
			channelVoices[i].Osc1Params.Shape = WaveShape::SQUARE;
			channelVoices[i].Osc2Params.Shape = WaveShape::SINE;
			channelVoices[i].Function = ENV_OUTPUT;
			channelVoices[i].Volume = 0.1;
		}
	}

	GameAudioManager::~GameAudioManager()
	{
	}

	//static
	void GameAudioManager::Init()
	{
		// Start the audio thread!
		mThread = CREATE_1(std::thread, &GameAudioManager::StartThread);
		while (!audioRunning) {}
	}

	//static
	void GameAudioManager::Uninit()
	{
		// Todo: Better cleanup
		audioRunning = false;
		mThread->join();
		DESTROY(mThread);
	}

	//static
	void GameAudioManager::StartThread()
	{
		instance = CREATE(GameAudioManager);
		instance->InitInternal();

		audioRunning = true;		

		while (audioRunning)
		{
			WaitForSingleObject(audioEvent, 1000);
			instance->Update();
		}

	}

	void GameAudioManager::CheckResult(const char* operation, long result)
	{
		if (result == 0) return;

		char buffer[64];
		snprintf(buffer, 64, "[Audio] %s: %ld", operation, result);
		DEBUG_LOG_COLOR(result == 0 ? Utils::Green : Utils::Red, buffer);
	}

	void GameAudioManager::LogFormat(WAVEFORMATEX* format)
	{
		char buffer[64];
		snprintf(buffer, 64, "[Audio] Channels: %d", format->nChannels);
		DEBUG_LOG_COLOR(Utils::Green, buffer);

		snprintf(buffer, 64, "[Audio] Samples Per Second: %d", format->nSamplesPerSec);
		DEBUG_LOG_COLOR(Utils::Green, buffer);

		snprintf(buffer, 64, "[Audio] Bits Per Sample: %d", format->wBitsPerSample);
		DEBUG_LOG_COLOR(Utils::Green, buffer);

		snprintf(buffer, 64, "[Audio] Format Tag: %d", format->wFormatTag);
		DEBUG_LOG_COLOR(Utils::Green, buffer);

		if (WAVE_FORMAT_EXTENSIBLE == format->wFormatTag)
		{
			DEBUG_LOG_COLOR(Utils::Green, "[Audio] Mix format is WAVE_FORMAT_EXTENSIBLE");

			WAVEFORMATEXTENSIBLE* formatEx = (WAVEFORMATEXTENSIBLE*)format;

			if (formatEx->SubFormat == KSDATAFORMAT_SUBTYPE_PCM)
			{
				DEBUG_LOG_COLOR(Utils::Green, "[Audio] Subformat: PCM");
			}

			if (formatEx->SubFormat == KSDATAFORMAT_SUBTYPE_IEEE_FLOAT)
			{
				DEBUG_LOG_COLOR(Utils::Green, "[Audio] Subformat: FLOAT");
			}
		}
	}

	void GameAudioManager::InitInternal()
	{
		srand((unsigned int)time(0));
		// Get us one of them there audio devices...

		// Todo: handle error cases instead of assuming all is well

		CoInitialize(NULL);

		DEBUG_LOG_COLOR(Utils::Cyan, "Initializing Audio");
		
		IMMDeviceEnumerator* enumerator;
		const CLSID enumeratorID = __uuidof(MMDeviceEnumerator);
		const CLSID ienumeratorID = __uuidof(IMMDeviceEnumerator);

		CheckResult("CoCreateInstance", CoCreateInstance(enumeratorID, NULL, CLSCTX_ALL, ienumeratorID, (void**)&enumerator));

		IMMDevice* audioEndpoint;
		CheckResult("GetDefaultAudioEndpoint", enumerator->GetDefaultAudioEndpoint(eRender, eConsole, &audioEndpoint));		

		CheckResult("audioEndpoint->Activate", audioEndpoint->Activate(__uuidof(IAudioClient), CLSCTX_ALL, NULL, (void**)&mAudioClient));

		CheckResult("audioClient->GetMixFormat", mAudioClient->GetMixFormat(&mWaveFormat));

		LogFormat(mWaveFormat);

		// Todo:  Figure out a proper buffer size...
		UINT32 requestedBufferSize = 500000;

		CheckResult("audioClient->Initialize", mAudioClient->Initialize(AUDCLNT_SHAREMODE_SHARED, AUDCLNT_STREAMFLAGS_EVENTCALLBACK, requestedBufferSize, 0, mWaveFormat, NULL));

		audioEvent = CreateEvent(0, false, false, "AudioEReadyEvent");
		CheckResult("Set Event Handle", mAudioClient->SetEventHandle(audioEvent));

		// Get the services we need:

		CheckResult("Get IAudioRenderClient Service", mAudioClient->GetService(__uuidof(IAudioRenderClient), (void**)&mRenderClient));

		//CheckResult("Get ISimpleAudioVolume Service", mAudioClient->GetService(__uuidof(ISimpleAudioVolume), (void**)&mVolumeService));

		IAudioSessionControl* sessionControl;
		CheckResult("Get IAudioSessionControl Service", mAudioClient->GetService(__uuidof(IAudioSessionControl), (void**)&sessionControl));
		sessionControl->SetDisplayName(L"Game Audio", NULL);

		UINT32 frameCount;
		mAudioClient->GetBufferSize(&frameCount);

		BYTE* buffer = nullptr;
		CheckResult("Get Buffer", mRenderClient->GetBuffer(frameCount, &buffer));

		// Fill initial buffer with 0s 
		UINT32 frameSize = mWaveFormat->nChannels * mWaveFormat->wBitsPerSample / 8; // Is this accurate?  Who knows!
		for (unsigned int i = 0; i < frameCount * frameSize; i++)
		{
			buffer[i] = 0;
		}

		CheckResult("Release buffer", mRenderClient->ReleaseBuffer(frameCount, AUDCLNT_BUFFERFLAGS_SILENT));

		CheckResult("Start Audio Client", mAudioClient->Start());

		Chain = CREATE(EffectChain); // Todo: move this somewhere else...

		Test(); // THIS IS NOT THREAD SAFE AT ALL
	}

	void GameAudioManager::UninitInternal()
	{
		// Todo!
	}

	void GameAudioManager::SetChannelVoiceInternal(int channel, VoiceParameters params)
	{
		channelVoices[channel] = params;
	}

	/*static*/ void  GameAudioManager::SetChannelVoice(int channel, VoiceParameters params)
	{
		instance->SetChannelVoiceInternal(channel, params);
	}

	void GameAudioManager::SetChannelVolumeInternal(int channel, float volume)
	{
		channelVoices[channel].Volume = volume;
	}

	/*static*/ void  GameAudioManager::SetChannelVolume(int channel, float volume)
	{
		instance->SetChannelVolumeInternal(channel, volume);
	}

	void GameAudioManager::SetMasterVolumeInternal(float volume)
	{
		mMasterVolume = volume;
	}

	/*static*/ void GameAudioManager::SetMasterVolume(float volume)
	{
		instance->SetMasterVolumeInternal(volume);
	}

	long long SoundID = 0;

	void GameAudioManager::Update()
	{
		auto iter = mPlayingSounds.begin();
		while (iter != mPlayingSounds.end())
		{
			auto instance = *iter;
			iter++;
			if (instance.Position >= instance.Sound->GetFrameCount())
			{
				mPlayingSounds.remove(instance);
			}
		}

		// Copy the queued sounds
		if (mMutex.try_lock())
		{
			for (Sound* sound : mSoundQueue)
			{
				mPlayingSounds.push_back({ sound, 0, SoundID });
				SoundID++;
			}

			for (MIDI* sound : mMIDIQueue)
			{
				mPlayingMIDI.push_back({ sound, 0, 0, SoundID, sound->GetLoop() });
				mPlayingMIDI.back().Start();
				SoundID++;
			}

			mMIDIQueue.clear();
			mSoundQueue.clear();
			mMutex.unlock();
		}

		UINT32 bufferSize;
		mAudioClient->GetBufferSize(&bufferSize);

		UINT32 padding;
		mAudioClient->GetCurrentPadding(&padding);

		UINT32 frameCount = bufferSize - padding;

		if (frameCount == 0)
		{
			return;
		}
		
		BYTE* buffer = nullptr;
		CheckResult("Get Buffer", mRenderClient->GetBuffer(frameCount, &buffer));

		if (buffer == nullptr)
		{
			DEBUG_LOG("Failed to get audio buffer");
			return;
		}

		if (!mMute)
		{
			FillPlaceholderAudioData(buffer, frameCount);
		}

		CheckResult("Release buffer", mRenderClient->ReleaseBuffer(frameCount, mMute ? AUDCLNT_BUFFERFLAGS_SILENT : 0));
	}

	void GameAudioManager::FillPlaceholderAudioData(unsigned char* data, int frameCount)
	{
		int index = 0;
		float sample = 0;
		BYTE* output = data;

		double dt = 1.0 / mWaveFormat->nSamplesPerSec;
		double fullSample = 0.0;

		while (index < frameCount)
		{
			fullSample = 0;

			for (PlayingMIDIInstance& playingInstance : mPlayingMIDI)
			{
				playingInstance.Step(dt);
			}

			for (auto voice : mVoices)
			{
				if (voice->IsPlaying())
				{
					voice->Step(dt);
					fullSample += voice->Sample();
				}
			}

			// Apply effect chain here... maybe this is wrong, who knows
			fullSample = Chain->Sample(fullSample, dt);

			for (int i = 0; i < mWaveFormat->nChannels; i++)
			{
				// Play sounds...
				if (i < 2)
				{
					for (const PlayingSoundInstance& playingInstance : mPlayingSounds)
					{
						if (playingInstance.Sound == nullptr || !playingInstance.Sound->IsLoaded())
						{
							continue;
						}

						if (playingInstance.Position < playingInstance.Sound->GetFrameCount())
						{
							unsigned int dataIndex = (playingInstance.Position * 2) + i;
							fullSample += playingInstance.Sound->GetSoundData()[dataIndex];
						}
					}					
				}

				sample = (float)fullSample * mMasterVolume;
				memcpy(output, &sample, sizeof(float));
				output += sizeof(float);
			}

			// Advance playing sounds
			for (PlayingSoundInstance& playingInstance : mPlayingSounds)
			{
				playingInstance.Position++;
			}

			index++;
		}
	}

	void GameAudioManager::NoteOn(unsigned char note)
	{
		instance->HandleMIDIEvent({
			0,
			NOTE_ON,
			0,
			0,
			note,
			0x00
			}, 1.0f);
	}

	void GameAudioManager::NoteOff(unsigned char note)
	{
		instance->HandleMIDIEvent({
			0,
			NOTE_OFF,
			0,
			0,
			note,
			0x00
			}, 1.0f);
	}

	unsigned int GameAudioManager::GetSampleRateInternal() const
	{
		return mWaveFormat->nSamplesPerSec;
	}

	void GameAudioManager::PlayInternal(Sound* sound)
	{
		mMutex.lock();
		mSoundQueue.push_back(sound);
		mMutex.unlock();
	}

	void GameAudioManager::PlayInternal(MIDI* sound, bool loop)
	{
		mMutex.lock();
		mMIDIQueue.push_back(sound);
		sound->SetLoop(loop);
		mMutex.unlock();
	}

	void GameAudioManager::HandleMIDIEventInternal(MIDIEvent evt, float instanceVolume)
	{
		switch (evt.Type)
		{
		case NOTE_ON:
			for (auto voice : mVoices)
			{
				if (!voice->IsPlaying())
				{	
					voice->SetParameters(channelVoices[evt.Channel]);					
					voice->NoteOn(evt.Data1);
					voice->SetInstanceVolume(instanceVolume);
					return;
				}
			}
			DEBUG_ERROR("Out of midi voices!");
			break;
		case NOTE_OFF:
			for (auto voice : mVoices)
			{
				if (voice->IsNoteOn() && voice->GetNote() == evt.Data1)
				{
					voice->NoteOff();
					return;
				}
			}
			DEBUG_ERROR("Failed to stop midi note....");
			break;
		default:
			break;
		}
	}

	/*static*/ void GameAudioManager::HandleMIDIEvent(MIDIEvent evt, float instanceVolume)
	{
		instance->HandleMIDIEventInternal(evt, instanceVolume);
	}

	/*static*/ unsigned int GameAudioManager::GetSampleRate() { return instance->GetSampleRateInternal(); }
	/*static*/ void GameAudioManager::Play(Sound* sound) { return instance->PlayInternal(sound); }
	/*static*/ void GameAudioManager::Play(MIDI* sound, bool loop) { return instance->PlayInternal(sound, loop); }

	/*static*/ void GameAudioManager::Test()
	{		
	}

	/*static*/ int GameAudioManager::SecondsToSamples(double time)
	{
		unsigned int rate = instance->GetSampleRateInternal();
		return (int)(rate * time);
	}
}