#ifndef LOW_PASS_H
#define LOW_PASS_H

#include "iEffect.h"

namespace Audio
{
	class LowPassEffect : public IEffect
	{
	private:
		int mSamplesToAverage;
		double* mBuffer;
		double mRunningTotal;
		int mIndex;
		int mNextIndex;

	public:
		LowPassEffect();
		~LowPassEffect();

		virtual double Sample(const double& input, const double& dt);
	};
}

#endif
