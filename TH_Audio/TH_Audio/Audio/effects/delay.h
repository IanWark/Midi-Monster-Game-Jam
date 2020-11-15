#ifndef DELAY_H
#define DELAY_H

#include "iEffect.h"

namespace Audio
{
	class DelayEffect : public IEffect
	{
	private:
		int mBufferSize;
		double* mBuffer;
		double mFeedback;
		int mIndex;
		int mNextIndex;

		double mWetMix;
		double mDryMix;

	public:
		DelayEffect();
		~DelayEffect();

		void SetDelayTime(double time);

		virtual double Sample(const double& input, const double& dt);
	};
}

#endif
