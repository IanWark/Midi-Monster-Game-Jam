#include "ADSREnvelope.h"

//#include "utils/file.h"

#include <iostream>
#include <vector>

namespace Audio
{
	ADSREnvelope::ADSREnvelope()
		: mValue(0)
		, mParameters()
		, mStage(ADSRStage::STOPPED)
	{
		mParameters.Attack = (0.01);
		mParameters.Decay = (0.08);
		mParameters.Sustain = (0.2);
		mParameters.Release = (0.8);
	}

	ADSREnvelope::~ADSREnvelope()
	{

	}

	void ADSREnvelope::Step(const double& dt)
	{
		switch (mStage)
		{
		case ADSRStage::ATTACK:
			StepAttack(dt);
			break;
		case ADSRStage::DECAY:
			StepDecay(dt);
			break;
		case ADSRStage::SUSTAIN:
			StepSustain(dt);
			break;
		case ADSRStage::RELEASE:
			StepRelease(dt);
			break;
		default:
			break;
		}
	}

	void ADSREnvelope::StepAttack(const double& dt)
	{
		mValue += dt / mParameters.Attack;
		if (mValue >= 1)
		{
			double remainder = (mValue - 1.0f) * dt;
			mValue = 1;
			mStage = ADSRStage::DECAY;
			Step(remainder);
		}
	}

	void ADSREnvelope::StepDecay(const double& dt)
	{
		mValue -= dt / mParameters.Decay;
		if (mValue <= mParameters.Sustain)
		{
			double remainder = (mParameters.Sustain - mValue) * dt;
			mValue = mParameters.Sustain;
			mStage = ADSRStage::SUSTAIN;
			Step(remainder);
		}
	}

	void ADSREnvelope::StepSustain(const double& dt)
	{
		//mValue = mSustain;
		// Potentially just do nothing?
	}

	void ADSREnvelope::StepRelease(const double& dt)
	{
		mValue -= dt / mParameters.Release;
		if (mValue <= 0)
		{
			mValue = 0;
			mStage = ADSRStage::STOPPED;
		}
	}

	double ADSREnvelope::Sample() const
	{
		return mValue;
	}

	void ADSREnvelope::Start()
	{
		mValue;
		mStage = ADSRStage::ATTACK;
	}

	void ADSREnvelope::Stop()
	{
		mStage = ADSRStage::RELEASE;
	}

	bool ADSREnvelope::IsDone() const 
	{		
		return mStage == ADSRStage::STOPPED;
	}

	void ADSREnvelope::SetAttack(const double& attack)
	{
		mParameters.Attack = attack;
	}
	
	void ADSREnvelope::SetDecay(const double& decay)
	{
		mParameters.Decay = decay;
	}

	void ADSREnvelope::SetSustain(const double& sustain)
	{
		mParameters.Sustain = sustain;
	}

	void ADSREnvelope::SetRelease(const double& release)
	{
		mParameters.Release = release;
	}

	void ADSREnvelope::SetParameters(ADSRParameters params)
	{
		mParameters = params;
	}

	void TestADSREnvelope()
	{
		ADSREnvelope envelope;

		std::vector<double> results;		

		double timer = 0.0;
		double totalTime = 0.0;
		double max = 1;
		double dt = 0.001;

		char doubleText[1024];

		envelope.Start();

		std::string resultString;

		while (!envelope.IsDone())
		{
			totalTime += dt;

			// Countdown until stopping the envelope
			if (timer < max)
			{
				timer += dt;
				if (timer >= max)
				{
					envelope.Stop();
				}
			}

			envelope.Step(dt);
			sprintf_s(doubleText, "%f", envelope.Sample());
			resultString += doubleText;
			resultString += ",";

			sprintf_s(doubleText, "%f", totalTime);

			resultString += doubleText;
			resultString += "\n";
		}

		//Utils::WriteTextToFile("adsr_result.txt", resultString.c_str());
	}
}