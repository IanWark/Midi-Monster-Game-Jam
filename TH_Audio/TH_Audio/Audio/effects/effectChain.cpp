#include "effectChain.h"

#include "iEffect.h"

namespace Audio
{
	EffectChain::EffectChain()
		: mEffects()
		, mWetMix(0.5)
		, mDryMix(0.8)
	{

	}

	EffectChain::~EffectChain()
	{
		for (IEffect* effect : mEffects)
		{
			DESTROY(effect);
		}
		mEffects.clear();
	}

	std::vector<IEffect*>::iterator iter;
	std::vector<IEffect*>::iterator end;
	double sample = 0; // static for performance reasons

	double EffectChain::Sample(const double& input, const double& dt)
	{
		sample = input;
		iter = mEffects.begin();
		end = mEffects.end();
		while (iter != end)
		{
			sample = (*iter)->Sample(sample, dt);
			iter++;
		}
		return (sample * mWetMix) + (input * mDryMix);
	}
}