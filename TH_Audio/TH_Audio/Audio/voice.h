#ifndef VOICE_H
#define VOICE_H

#include "ADSREnvelope.h"
#include "oscillator.h"

namespace Audio
{
	typedef unsigned char Note;

	enum EnvelopeFunction : int
	{
		ENV_OUTPUT =		1 << 0,
		ENV_OUTPUT_OSC_1 =	1 << 1,
		ENV_OUTPUT_OSC_2 =	1 << 2,
		ENV_NOTE =			1 << 3,
		ENV_NOTE_OSC_1 =	1 << 4,
		ENV_NOTE_OSC_2 =	1 << 5,
	};

	struct VoiceParameters
	{
		OscillatorParameters Osc1Params;
		OscillatorParameters Osc2Params;
		ADSRParameters ADSRParams;
		int Function;
		double Volume;
	};

	class Voice
	{
	private:
		Oscillator mOscillator1;
		Oscillator mOscillator2;
		ADSREnvelope mEnvelope;

		VoiceParameters mParameters;

		Note mNote;

		double mSample;
		float mInstanceVolume;
	public:
		Voice();
		~Voice();

		void Step(const double& dt);

		double Sample() const;

		void NoteOn(Note note);
		void NoteOff();

		bool IsNoteOn() const;
		bool IsPlaying() const;

		void SetParameters(VoiceParameters params);

		void SetInstanceVolume(float volume);

		Note GetNote() const { return mNote; }

		static void InitFrequencyMap();
	};
}

#endif