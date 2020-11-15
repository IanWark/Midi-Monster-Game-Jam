#include "sound.h"

#include "../Memory/memory.h"
//#include "tasks/soundLoadTask.h"
#include "../GameUtil/debugConsole.h"
//#include "../GameUtil/taskManager.h"
#include "audioManager.h"

//#include "stb_vorbis/stb_vorbis.h"
//#include "secretrabbitcode/samplerate.h"

namespace Audio
{
	Sound::Sound()
		: mIsAsync(false)
		, mIsReady(false)
		, mMutex()
		, mPath()
	{

	}

	/*virtual*/ Sound::~Sound()
	{

	}

	bool Sound::LoadInternal(const char* path)
	{
		//int outError = 0;
		//stb_vorbis* vorbisFile = stb_vorbis_open_filename(path, &outError, nullptr);

		//if (vorbisFile == nullptr)
		//{
		//	DEBUG_ERROR("[Sound] Failed to open vorbis file.");
		//	return false;
		//}

		//stb_vorbis_info info = stb_vorbis_get_info(vorbisFile);

		//mFrameCount = stb_vorbis_stream_length_in_samples(vorbisFile);
		//unsigned int wanted_channels = 2;
		//unsigned int buffer_size = mFrameCount * wanted_channels;

		//mSoundData = CREATE_ARRAY(float, buffer_size);

		//int result = stb_vorbis_get_samples_float_interleaved(vorbisFile, 2, mSoundData, buffer_size);

		//ASSERT(result == mFrameCount, "[Sound] Incorrect number of samples per channel.");

		//// RESAMPLE!
		//unsigned int desiredSampleRate = GameAudioManager::GetSampleRate();
		//if (desiredSampleRate != info.sample_rate)
		//{
		//	double ratio = (double)desiredSampleRate / (double)info.sample_rate;
		//	int new_samples_per_channel = (int)(mFrameCount * ratio);

		//	SRC_DATA conversionData;
		//	conversionData.src_ratio = ratio;

		//	conversionData.data_in = mSoundData;
		//	conversionData.input_frames = mFrameCount;

		//	conversionData.data_out = CREATE_ARRAY(float, new_samples_per_channel * wanted_channels);
		//	conversionData.output_frames = new_samples_per_channel;

		//	result = src_simple(&conversionData, SRC_SINC_BEST_QUALITY, 2);

		//	// Keep only the resampled version...
		//	DESTROY_ARRAY(mSoundData);
		//	mSoundData = conversionData.data_out;
		//	mFrameCount = conversionData.output_frames;
		//}

		return true;
	}

	bool Sound::UnloadInternal()
	{
		DESTROY_ARRAY(mSoundData);
		return false;
	}

	bool Sound::ReloadInternal()
	{
		return false;
	}

	/*virtual*/ void Sound::Load(const char* path)
	{
		mPath = path;
		mIsReady = LoadInternal(path);
	}

	/*virtual*/ void Sound::LoadAsync(const char* aFilename)
	{
		//mPath = aFilename;
		//mIsAsync = true;

		//mTaskParams = CREATE(Tasks::SoundLoadTaskParams);
		//mTaskParams->sound = this;
		//mTaskParams->path = aFilename;
		//mTaskParams->callback = [](Sound* self) {
		//	self->mMutex.lock();
		//	self->mIsReady = true; // Is this always true here...?
		//	self->mMutex.unlock();
		//	DESTROY(self->mTaskParams);
		//	self->mTaskParams = 0;
		//};

		//GAMETASKS->RunTaskAsync<Tasks::SoundLoadTask>(mTaskParams);
	}

	/*virtual*/ void Sound::Unload() { UnloadInternal(); }
	/*virtual*/ void Sound::UnloadAsync() { UnloadInternal(); }
	/*virtual*/ void Sound::Reload() 
	{
		UnloadInternal();
		LoadInternal(mPath.c_str());
	}

	/*virtual*/ void Sound::ReloadAsync() {}
	/*virtual*/ bool Sound::IsLoaded()
	{
		if (!mIsAsync)
		{
			return mIsReady;
		}

		if (mMutex.try_lock())
		{
			bool result = mIsReady;
			mMutex.unlock();
			return result;
		}
		return false;
	}
}