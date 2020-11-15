#include "midi.h"

#include "../Memory/memory.h"
//#include "tasks/midiLoadTask.h"
#include "../GameUtil/debugConsole.h"
//#include "utils/taskManager.h"
#include "audioManager.h"

#include <algorithm>

namespace Audio
{
	//////// HELPERS /////////////////////////

	size_t fread_swapped_16(void* buffer, FILE* stream)
	{
		size_t result = fread(buffer, 2, 1, stream);
		int16_t* typedBuffer = (int16_t*)buffer;
		*typedBuffer = _byteswap_ushort(*typedBuffer);
		return result;
	}

	size_t fread_swapped_32(void* buffer, FILE* stream)
	{
		size_t result = fread(buffer, 4, 1, stream);
		int32_t* typedBuffer = (int32_t*)buffer;
		*typedBuffer = _byteswap_ulong(*typedBuffer);
		return result;
	}

	int read_variable_length(FILE* stream, int* out)
	{
		ASSERT(out != nullptr, "Out value can't be nullptr");
		*out = 0;
		int readCount = 0;
		unsigned char part = 0;

		do
		{
			readCount++;

			*out = *out << 7;
			fread(&part, 1, 1, stream);
			*out |= part & 127;
		} while ((part & 1 << 7) != 0);

		return readCount;
	}

	struct MIDIHeader
	{
		char MThd[4];
		uint32_t header_length;
		uint16_t format;
		uint16_t chunk_count;
		int16_t division;
	};

	struct TrackChunkHeader
	{
		char MTrk[4];
		uint32_t length;
	};

	//////////////////////// MIDI CLASS STARTS HERE ///////////////////////

	MIDI::MIDI()
		: mIsAsync(false)
		, mIsReady(false)
		, mLoop(false)
		, mPulsePerQuarterNote(2000)
		, mUSecondPerQuarterNote(0)
		, mBPM(120)
		, mTicksPerSecond(0.0)
		, mInstanceVolume(1.0f)
		, mMutex()
		, mPath()
	{

	}

	/*virtual*/ MIDI::~MIDI()
	{

	}

	bool MIDI::LoadInternal(const char* path)
	{
		FILE* file;
		fopen_s(&file, path, "rb");

		if (file == nullptr)
		{
			DEBUG_ERROR("Failed to open file");
			return false;
		}

		MIDIHeader header;
		fread(&header.MThd, sizeof(char) * 4, 1, file);
		fread_swapped_32(&header.header_length, file);
		fread_swapped_16(&header.format, file);
		fread_swapped_16(&header.chunk_count, file);
		fread_swapped_16(&header.division, file);

		mPulsePerQuarterNote = header.division;

		ASSERT(header.header_length == 6, "MIDI header is malformed.");

		for (int i = 0; i < header.chunk_count; i++)
		{
			// Start each track at time 0
			mMIDIEventData.push_back({ 0, (MIDIEventType)0, i, 0, 0, 0 });

			TrackChunkHeader trackHeader;
			fread(&trackHeader.MTrk, sizeof(char) * 4, 1, file);
			fread_swapped_32(&trackHeader.length, file);

			int remaining = trackHeader.length;
			unsigned char runningStatus = 0;

			while (remaining > 0)
			{
				remaining -= ReadEvent(file, i);
			}
		}

		std::sort(mMIDIEventData.begin(), mMIDIEventData.end(), [](const MIDIEvent& one, const MIDIEvent& two) { return one.Time < two.Time; });
		
		mBPM = 60000000.0 / mUSecondPerQuarterNote;
		mTicksPerSecond = (mBPM / 60.0) * mPulsePerQuarterNote;

		fclose(file);
		return true;
	}

	int MIDI::ReadEvent(FILE* file, int trackIndex)
	{
		int event_time;
		int readCount = read_variable_length(file, &event_time);

		if (event_time != 0)
		{
			int prevTime = mMIDIEventData.size() > 0 ? mMIDIEventData.back().Time : 0;
			mMIDIEventData.push_back({ event_time + prevTime, (MIDIEventType)0, trackIndex, 0, 0, 0 });
		}
				
		unsigned char messageId = 0;
		fread(&messageId, sizeof(char), 1, file);
		readCount++;

		switch (messageId)
		{
		case 0xFF:
			readCount += ReadMetaEvent(file, trackIndex);
			break;
		case 0xF7:
			readCount += ReadSysexEvent(file, trackIndex);
			break;
		default:
			readCount += ReadMIDIEvent(file, messageId, trackIndex);
			break;
		}

		return readCount;
	}

	int  MIDI::ReadMIDIEvent(FILE* file, unsigned char id, int trackIndex)
	{
		int readCount = 0;

		unsigned char eventType = id & 0xF0;
		unsigned char channel = id & 0x0F;
		unsigned char data1 = 0;
		unsigned char data2 = 0;

		//unsigned char lookAhead = 0;
		//size_t lookAheadRead = 0;

		//do
		//{
			switch (eventType)
			{
			case 0x80:
			case 0x90:
			case 0xA0:
			case 0xE0:
			case 0xB0:
				// Most events have two data bytes
				readCount += (int)fread(&data1, sizeof(char), 1, file);
				readCount += (int)fread(&data2, sizeof(char), 1, file);
				break;
			case 0xC0:
			case 0xD0:
				// These two events have one data byte
				readCount += (int)fread(&data1, sizeof(char), 1, file);
				break;
			default:
				// What is this?
				break;
			}

			int prevTime = mMIDIEventData.size() > 0 ? mMIDIEventData.back().Time : 0;
			mMIDIEventData.push_back({ prevTime, (MIDIEventType)eventType, channel, trackIndex, data1, data2 });

			// This might not be needed, but we might run into running status
			//lookAheadRead = fread(&lookAhead, 1, 1, file);
			//fseek(file, -1, SEEK_CUR);
		//} while (lookAheadRead > 0 && lookAhead < 0x80);

		return readCount;
	}

	int  MIDI::ReadSysexEvent(FILE* file, int trackIndex)
	{
		// TODO!  Sysex messages will break this!
		return 0;
	}

	int  MIDI::ReadMetaEvent(FILE* file, int trackIndex)
	{		
		unsigned char metaEventType = 0;
		fread(&metaEventType, sizeof(char), 1, file);

		int length;
		int readCount = read_variable_length(file, &length);

		unsigned char* buffer = CREATE_ARRAY(unsigned char, length);

		fread(buffer, sizeof(char) * length, 1, file);

		switch (metaEventType)
		{
		case 0x51: // TEMPO!
			mUSecondPerQuarterNote = 0 | buffer[0] << 16 | buffer[1] << 8 | buffer[2];
			break;
		default:
			break;
		}

		DESTROY_ARRAY(buffer);

		return readCount + length + 1;
	}

	bool MIDI::UnloadInternal()
	{
		return false;
	}

	bool MIDI::ReloadInternal()
	{
		return false;
	}

	/*virtual*/ void MIDI::Load(const char* path)
	{
		mPath = path;
		mIsReady = LoadInternal(path);
	}

	/*virtual*/ void MIDI::LoadAsync(const char* aFilename)
	{
		//mPath = aFilename;
		//mIsAsync = true;

		//mTaskParams = CREATE(Tasks::MIDILoadTaskParams);
		//mTaskParams->sound = this;
		//mTaskParams->path = aFilename;
		//mTaskParams->callback = [](MIDI* self) {
		//	self->mMutex.lock();
		//	self->mIsReady = true; // Is this always true here...?
		//	self->mMutex.unlock();
		//	DESTROY(self->mTaskParams);
		//	self->mTaskParams = 0;
		//};

		//GAMETASKS->RunTaskAsync<Tasks::MIDILoadTask>(mTaskParams);
	}

	/*virtual*/ void MIDI::Unload() { UnloadInternal(); }
	/*virtual*/ void MIDI::UnloadAsync() { UnloadInternal(); }
	/*virtual*/ void MIDI::Reload()
	{
		UnloadInternal();
		LoadInternal(mPath.c_str());
	}

	/*virtual*/ void MIDI::ReloadAsync() {}
	/*virtual*/ bool MIDI::IsLoaded()
	{
		if (!mIsAsync)
		{
			return mIsReady;
		}

		if (mMutex.try_lock())
		{
			bool result = mIsReady;
			mMutex.unlock();
			return result;
		}
		return false;
	}

	void MIDI::SetBPM(double bpm)
	{
		mBPM = bpm;
		mTicksPerSecond = (mBPM / 60.0) * mPulsePerQuarterNote;
	}

	void MIDI::SetVolume(float volume)
	{
		mInstanceVolume = volume;
	}
}