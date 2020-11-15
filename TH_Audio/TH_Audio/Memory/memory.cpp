#include "memory.h"

#include "../GameUtil/debugConsole.h"

#include <cstdio>

namespace Memory
{
	MemTracker Tracker;

	MemTracker::MemTracker()
	{

	}

	MemTracker::~MemTracker()
	{
	}

	void* MemTracker::TrackAlloc(void* ptr, const char* type, size_t size, const char* file, int line, int arrayCount)
	{
		if (size != 0)
		{
			mAllocMap[ptr] = { type, size, file, line, arrayCount };
		}
		return ptr;
	}

	void* MemTracker::TrackFree(void* ptr)
	{
		mAllocMap.erase(ptr);
		return ptr;
	}

	int MemTracker::GetAllocCount()
	{
		return (int)mAllocMap.size();
	}

	int MemTracker::GetAllocSize()
	{
		int result = 0;
		for (auto& alloc : mAllocMap)
		{
			result += (int)alloc.second.size;
		}
		return result;
	}

	void MemTracker::Report()
	{
		const int textLength = 1024;
		char text[textLength];

		DEBUG_LOG("\nMEMORY REPORT\n");

		snprintf(text, textLength, "Total Objects: %d", GetAllocCount());
		DEBUG_LOG(text);

		snprintf(text, textLength, "Total Size: %d bytes", GetAllocSize());
		DEBUG_LOG(text);

		DEBUG_LOG("");

		std::map<std::string, ReportTypeDetails> mSizePerType;

		for (auto& alloc : mAllocMap)
		{
			int count = alloc.second.arrayCount > 0 ? alloc.second.arrayCount : 1;
			if (mSizePerType.find(alloc.second.type) == mSizePerType.end())
			{
				mSizePerType[alloc.second.type] = { count, (int)alloc.second.size };
			}
			else
			{
				mSizePerType[alloc.second.type].count += count;
				mSizePerType[alloc.second.type].size += (int)alloc.second.size;
			}
		}

		for (auto& type : mSizePerType)
		{
			snprintf(text, textLength, "%s > %d (%d)", type.first.c_str(), type.second.count, type.second.size);
			DEBUG_LOG(text);
		}
	}

}