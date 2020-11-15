#include "voice.h"

#include <cmath>

namespace Audio
{
	double MIDI_FREQUENCY_MAP[200];	

	/*static*/ void Voice::InitFrequencyMap()
	{
		for (double i = 0; i < 200.0; i++)
		{
			MIDI_FREQUENCY_MAP[(int)i] = 27.5 * pow(2.0, ((i - 21.0) / 12.0));
		}
	}

	Voice::Voice()
		: mOscillator1()
		, mOscillator2()
		, mEnvelope()
		, mParameters()
		, mNote(0)
		, mSample(0)
		, mInstanceVolume(1.0f)
	{
	}

	Voice::~Voice()
	{
	}

	void Voice::Step(const double& dt)
	{
		mOscillator1.Step(dt);
		mOscillator2.Step(dt);
		mEnvelope.Step(dt);

		if (mParameters.Function & (ENV_NOTE | ENV_NOTE_OSC_1))
		{
			mOscillator1.SetFrequency(MIDI_FREQUENCY_MAP[mNote] * (mEnvelope.Sample()));
		}
		if (mParameters.Function & (ENV_NOTE | ENV_NOTE_OSC_2))
		{
			mOscillator2.SetFrequency(MIDI_FREQUENCY_MAP[mNote] * (mEnvelope.Sample()));
		}

		if (mParameters.Function & ENV_OUTPUT_OSC_1)
		{
			mSample = mOscillator1.Sample() * mEnvelope.Sample();
		}
		else
		{
			mSample = mOscillator1.Sample();
		}

		if (mParameters.Function & ENV_OUTPUT_OSC_2)
		{
			mSample += mOscillator2.Sample() * mEnvelope.Sample();
		}
		else
		{
			mSample += mOscillator2.Sample();
		}			

		if (mParameters.Function & ENV_OUTPUT)
		{
			mSample *= mEnvelope.Sample();
		}

		mSample *= mParameters.Volume * mInstanceVolume;
		mSample *= 0.5; // Halving the output because of using two osc
	}

	double Voice::Sample() const
	{
		return mSample;
	}

	void Voice::NoteOn(Note note)
	{
		mEnvelope.Start();
		mNote = note;
		mOscillator1.Reset();
		mOscillator2.Reset();
		mOscillator1.SetFrequency(MIDI_FREQUENCY_MAP[note]);
		mOscillator2.SetFrequency(MIDI_FREQUENCY_MAP[note]);
	}

	void Voice::NoteOff()
	{
		mEnvelope.Stop();
	}

	void Voice::SetParameters(VoiceParameters params)
	{		
		mOscillator1.SetParameters(params.Osc1Params);
		mOscillator2.SetParameters(params.Osc2Params);
		mEnvelope.SetParameters(params.ADSRParams);
		mParameters = params;
	}

	bool Voice::IsNoteOn() const
	{
		return mEnvelope.GetCurrentState() < ADSRStage::RELEASE;
	}

	bool Voice::IsPlaying() const
	{
		return !mEnvelope.IsDone();
	}

	void Voice::SetInstanceVolume(float volume)
	{
		mInstanceVolume = volume;
	}
}