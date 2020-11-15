#ifndef MEMORY_H
#define MEMORY_H

#include <map>
#include <typeinfo>

// Todo: release version
#define CREATE(type) (type*)Memory::Tracker.TrackAlloc(new type(), typeid(type).name(), sizeof(type), __FILE__, __LINE__)
#define CREATE_1(type, args1) (type*)Memory::Tracker.TrackAlloc(new type(args1), typeid(type).name(), sizeof(type), __FILE__, __LINE__)
#define CREATE_2(type, args1, args2) (type*)Memory::Tracker.TrackAlloc(new type(args1, args2), typeid(type).name(), sizeof(type), __FILE__, __LINE__)
#define CREATE_3(type, args1, args2, args3) (type*)Memory::Tracker.TrackAlloc(new type(args1, args2, args3), typeid(type).name(), sizeof(type), __FILE__, __LINE__)
#define CREATE_4(type, args1, args2, args3, args4) (type*)Memory::Tracker.TrackAlloc(new type(args1, args2, args3, args4), typeid(type).name(), sizeof(type), __FILE__, __LINE__)
#define CREATE_ARRAY(type, size) (type*)Memory::Tracker.TrackAlloc(new type[size], typeid(type).name(), sizeof(type) * size, __FILE__, __LINE__, size)
#define DESTROY(item) delete item; Memory::Tracker.TrackFree(item); item = nullptr;
#define DESTROY_ARRAY(item) delete[] item; Memory::Tracker.TrackFree(item); item = nullptr;

namespace Memory
{
	struct AllocInfo
	{
		const char* type;
		size_t size;
		const char* file;
		int line;
		int arrayCount;
	};

	struct ReportTypeDetails
	{
		int count;
		int size;
		int arrayCount;
	};

	class MemTracker
	{
	protected:
		std::map<void*, AllocInfo> mAllocMap;
	public:
		MemTracker();
		~MemTracker();

		int GetAllocCount();
		int GetAllocSize();

		void* TrackAlloc(void* ptr, const char* type, size_t size, const char* file, int line, int arrayCount = 0);
		void* TrackFree(void* ptr);

		void Report();
	};

	extern MemTracker Tracker;
}

#endif