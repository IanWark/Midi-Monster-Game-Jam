#ifndef SOUND_H
#define SOUND_H

//#include "utils/resourceManager.h"

#include <mutex>

namespace Tasks
{
	class SoundLoadTask;
	struct SoundLoadTaskParams;
}

namespace Audio
{
	class Sound// : public Utils::IResource
	{
	private:		
		bool mIsAsync;
		bool mIsReady;

		float* mSoundData;
		unsigned int mFrameCount;

		std::mutex mMutex;
		Tasks::SoundLoadTaskParams* mTaskParams;

		std::string mPath;
	
		bool LoadInternal(const char* path);
		bool UnloadInternal();
		bool ReloadInternal();

		Sound();
		virtual ~Sound();
	public:
		// From Utils::IResource
		virtual void Load(const char*);
		virtual void LoadAsync(const char*);
		virtual void Unload();
		virtual void UnloadAsync();
		virtual void Reload();
		virtual void ReloadAsync();
		virtual bool IsLoaded();

		float* GetSoundData() const { return mSoundData; }
		unsigned int GetFrameCount() const { return mFrameCount; }

		//friend class Tasks::SoundLoadTask;
		//friend class Utils::ResourceManager;
	};
}

#endif