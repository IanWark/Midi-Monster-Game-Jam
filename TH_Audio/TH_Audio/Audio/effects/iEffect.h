#ifndef I_EFFECT_H
#define I_EFFECT_H

namespace Audio
{
	class IEffect
	{
	public:
		virtual double Sample(const double& input, const double& dt) = 0;
	};
}

#endif
