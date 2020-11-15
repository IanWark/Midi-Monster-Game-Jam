#include "lowPass.h"

#include "../../Memory/memory.h"

namespace Audio
{
	LowPassEffect::LowPassEffect()
		: mSamplesToAverage(30)
		, mBuffer(nullptr)
		, mRunningTotal(0)
		, mIndex(0)
		, mNextIndex(1)
	{
		mBuffer = CREATE_ARRAY(double, mSamplesToAverage);
		memset(mBuffer, 0, sizeof(double) * mSamplesToAverage);
	}

	LowPassEffect::~LowPassEffect()
	{
		DESTROY_ARRAY(mBuffer);
	}

	double LowPassEffect::Sample(const double& input, const double& dt)
	{
		mBuffer[mIndex] = input;
		mRunningTotal += input;
		mRunningTotal -= mBuffer[mNextIndex];

		mIndex = mNextIndex;
		mNextIndex++;
		if (mNextIndex == mSamplesToAverage)
		{
			mNextIndex = 0;
		}

		return mRunningTotal / mSamplesToAverage;
	}
}