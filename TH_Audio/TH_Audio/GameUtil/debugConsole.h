#ifndef DEBUG_CONSOLE_H
#define DEBUG_CONSOLE_H

#include <string>

namespace Utils
{
	enum DebugColor {
		White = 0,
		Black,
		Grey,
		Red,
		Green,
		Blue,
		Yellow,
		Cyan
	};

	class DebugConsole
	{
	protected:
		void* mOutHandle;
		void* mInHandle;
		DebugColor mCurrentColor;
		DebugColor mPreviousColor;

		std::string mInputText;

	public:
		DebugConsole();
		~DebugConsole();
		void Init();
		void Log(char);
		void Log(const char*);
		void Log(float);
		void Log(int);
		void Log(void*);
		void Log(std::string);
		void SetColor(DebugColor);
		void ResetColor();

		void Update();

		void LogLine(const char* t) { Log(t); Log("\n"); }
		void LogLine(std::string t) { Log(t); Log("\n"); }
	};

	void Assert(bool condition, const char* conditionText, const char* file, int line, const char* message = nullptr);

	void DebugDoNothing();
}

// Assert STAYS in the game in release mode...
#define ASSERT(condition, msg) Utils::Assert(condition, #condition, __FILE__, __LINE__, msg);


#if _DEBUG

#define DEBUG_WARN(value) //DEBUG_LOG_COLOR(Utils::Yellow, value);
#define DEBUG_ERROR(value) //DEBUG_LOG_COLOR(Utils::Red, value);
#define DEBUG_COLOR(clr) //GAMEENGINE->GetConsole()->SetColor(clr);
#define DEBUG_LOG(value) //GAMEENGINE->GetConsole()->LogLine(value);
#define DEBUG_LOG_COLOR(clr, value)/* \
GAMEENGINE->GetConsole()->SetColor(clr); \
GAMEENGINE->GetConsole()->LogLine(value); \
GAMEENGINE->GetConsole()->ResetColor();*/

#else

#define DEBUG_WARN(value) Utils::DebugDoNothing();
#define DEBUG_ERROR(value) Utils::DebugDoNothing();
#define DEBUG_COLOR(clr) Utils::DebugDoNothing();
#define DEBUG_LOG(value) Utils::DebugDoNothing();
#define DEBUG_LOG_COLOR(clr, value) Utils::DebugDoNothing();

#endif

#endif