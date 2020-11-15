#ifndef OSCILLATOR_H
#define OSCILLATOR_H

#include "ADSREnvelope.h"

namespace Audio
{
	enum class WaveShape : int
	{
		OFF = 0,
		SINE = 1,
		SQUARE = 2,
		TRIANGLE = 3	
	};

	struct OscillatorParameters
	{
		WaveShape Shape;
	};

	class Oscillator
	{
	private:
		OscillatorParameters mParameters;

		double mFrequency;

		double mTime;
		double mSample;

		double Sample(const double& time);
	public:

		Oscillator();
		~Oscillator();

		void Step(const double& dt);
		double Sample() const;

		void Reset();

		void SetFrequency(double freq);
		void SetShape(WaveShape shape);

		void SetParameters(OscillatorParameters params);

		double GetFrequency() const { return mFrequency; }
	};
}

#endif