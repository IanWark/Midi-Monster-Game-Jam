#ifndef EFFECT_CHAIN_H
#define EFFECT_CHAIN_H

#include <vector>

#include "../../Memory/memory.h"
#include "../../GameUtil/debugConsole.h"

namespace Audio
{
	class IEffect;

	class EffectChain
	{
	private:

		std::vector<IEffect*> mEffects;

		double mWetMix;
		double mDryMix;

	public:
		EffectChain();
		~EffectChain();

		template <typename T>
		IEffect* AddEffect()
		{
			T* object = CREATE(T);
			IEffect* effect = dynamic_cast<T*>(object);
			ASSERT(effect != nullptr, "Failed to create an IEffect");
			mEffects.push_back(effect);
			return effect;
		}

		double Sample(const double& input, const double& dt);
	};
}

#endif