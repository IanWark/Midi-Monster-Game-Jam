#include "oscillator.h"

#include <algorithm>

namespace Audio
{
	const double PI = 4.0 * atan(1.0);
	const double TWOPI = 8.0 * atan(1.0);

	Oscillator::Oscillator()
		: mParameters()
		, mFrequency(440.0) // Default is 440hz = A
		, mTime(0.0)
		, mSample(0.0)	
	{
		mParameters.Shape = WaveShape::SINE;
	}

	Oscillator::~Oscillator()
	{

	}

	double sine;

	double Oscillator::Sample(const double& time)
	{
		switch (mParameters.Shape)
		{
		case WaveShape::SQUARE:
			return (sin(TWOPI * time) > 0 ? 1.0 : -1.0);
		case WaveShape::SINE:
			return sin(TWOPI * time);		
		default:
			return 0;
		}
	}

	double Oscillator::Sample() const
	{
		return mSample;
	}

	void Oscillator::Reset()
	{
		mTime = 0;
		mSample = 0;
	}

	void Oscillator::Step(const double& dt)
	{
		mTime += dt * mFrequency;
		mSample = Sample(mTime);// *mEnvelope.Sample();
	}

	void Oscillator::SetFrequency(double freq)
	{		
		while (mTime > 10.0)
		{
			mTime -= 10.0;
		}
		mFrequency = freq;
	}

	void Oscillator::SetShape(WaveShape shape)
	{
		mParameters.Shape = shape;
	}

	void Oscillator::SetParameters(OscillatorParameters params)
	{
		mParameters = params;
	}
}