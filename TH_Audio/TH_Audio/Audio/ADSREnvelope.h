#ifndef ADSR_ENVELOPE_H
#define ADSR_ENVELOPE_H

namespace Audio
{
	enum class ADSRStage : short
	{
		ATTACK,
		DECAY,
		SUSTAIN,
		RELEASE,
		STOPPED
	};

	struct ADSRParameters
	{
		double Attack;
		double Decay;
		double Sustain;
		double Release;
	};

	class ADSREnvelope
	{
	private:
		double mValue;

		ADSRParameters mParameters;

		ADSRStage mStage;

		void StepAttack(const double& dt);
		void StepDecay(const double& dt);
		void StepSustain(const double& dt);
		void StepRelease(const double& dt);

	public:

		ADSREnvelope();
		~ADSREnvelope();

		void Step(const double& dt);

		double Sample() const;

		void SetAttack(const double& attack);
		void SetDecay(const double& decay);
		void SetSustain(const double& sustain);
		void SetRelease(const double& release);

		void SetParameters(ADSRParameters params);

		ADSRStage GetCurrentState() const { return mStage; }

		void Start();
		void Stop();

		bool IsDone() const;
	};

	void TestADSREnvelope();
}

#endif