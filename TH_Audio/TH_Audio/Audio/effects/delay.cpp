#include "delay.h"

#include "../../Memory/memory.h"

#include "../audioManager.h"

namespace Audio
{
	DelayEffect::DelayEffect()
		: mBufferSize(7000)
		, mBuffer(nullptr)
		, mFeedback(0.5)
		, mIndex(0)
		, mNextIndex(1)
		, mWetMix(1)
		, mDryMix(0)
	{
		mBuffer = CREATE_ARRAY(double, mBufferSize);
		memset(mBuffer, 0, sizeof(double) * mBufferSize);
	}

	DelayEffect::~DelayEffect()
	{
		DESTROY_ARRAY(mBuffer);
	}

	double DelayEffect::Sample(const double& input, const double& dt)
	{
		mBuffer[mIndex] *= mFeedback;
		mBuffer[mIndex] += input;		

		mIndex = mNextIndex;
		mNextIndex++;

		if (mNextIndex == mBufferSize)
		{
			mNextIndex = 0;
		}

		return (mBuffer[mIndex] * mWetMix) + (input * mDryMix);
	}

	void DelayEffect::SetDelayTime(double time)
	{
		DESTROY_ARRAY(mBuffer);
		mBufferSize = GameAudioManager::SecondsToSamples(time);
		mBuffer = CREATE_ARRAY(double, mBufferSize);
		memset(mBuffer, 0, sizeof(double) * mBufferSize);
	}
}