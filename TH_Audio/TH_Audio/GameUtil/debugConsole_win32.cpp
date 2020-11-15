#include "debugConsole.h"

#if _WIN32
#include <Windows.h>

#include <string>
#include <sstream>
#include <iostream>
#include <csignal>

namespace Utils
{

	using namespace std;

	DebugConsole::DebugConsole()
	{
#ifdef _DEBUG
		AllocConsole();
		mOutHandle = GetStdHandle(STD_OUTPUT_HANDLE);
		SetConsoleTitleA("TH AUDIO DEBUG CONSOLE");

		mInHandle = GetStdHandle(STD_INPUT_HANDLE);

		CONSOLE_CURSOR_INFO cursorInfo;
		cursorInfo.bVisible = true;
		cursorInfo.dwSize = sizeof(CONSOLE_CURSOR_INFO);

		SetConsoleMode(mInHandle, ENABLE_WINDOW_INPUT | ENABLE_MOUSE_INPUT);
		SetConsoleCursorInfo(mInHandle, &cursorInfo);

		mCurrentColor = White;
		mPreviousColor = White;
#endif
	}

	DebugConsole::~DebugConsole()
	{
#ifdef _DEBUG
		FreeConsole();
#endif
	}

	void DebugConsole::Init()
	{
#ifdef _DEBUG
		mInputText = "";
#endif
	}

	void DebugConsole::Update()
	{
	}

	void DebugConsole::Log(const char* aText)
	{
#ifdef _DEBUG
		string t = "";
		t += aText;
		WriteConsoleA(mOutHandle, t.c_str(), (DWORD)t.size(), 0, 0);
#endif
	}

	void DebugConsole::Log(char aText)
	{
#ifdef _DEBUG
		WriteConsoleA(mOutHandle, &aText, 1, 0, 0);
#endif
	}

	void DebugConsole::Log(std::string aText)
	{
#ifdef _DEBUG
		WriteConsoleA(mOutHandle, aText.c_str(), (DWORD)aText.size(), 0, 0);
#endif
	}

	void DebugConsole::SetColor(DebugColor aColor)
	{
#ifdef _DEBUG
		static const int colors[] = {
			FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE | FOREGROUND_INTENSITY,
			0,
			FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE,
			FOREGROUND_RED | FOREGROUND_INTENSITY,
			FOREGROUND_GREEN | FOREGROUND_INTENSITY,
			FOREGROUND_BLUE | FOREGROUND_INTENSITY,
			FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_INTENSITY,
			FOREGROUND_BLUE | FOREGROUND_GREEN | FOREGROUND_INTENSITY
		};

		SetConsoleTextAttribute(mOutHandle, colors[aColor]);
		mPreviousColor = mCurrentColor;
		mCurrentColor = aColor;
#endif
	}

	void DebugConsole::ResetColor()
	{
#ifdef _DEBUG
		SetColor(mPreviousColor);
#endif
	}

	void DebugConsole::Log(int aNumber)
	{
#ifdef _DEBUG
		std::stringstream s;
		s << aNumber;
		Log(s.str());
#endif
	}

	void DebugConsole::Log(float aNumber)
	{
#ifdef _DEBUG
		std::stringstream s;
		s << aNumber;
		Log(s.str());
#endif
	}

	void DebugConsole::Log(void* aNumber)
	{
#ifdef _DEBUG
		std::stringstream s;
		s << aNumber;
		Log(s.str());
#endif
	}

	void DebugDoNothing()
	{
		// Do nothing!
	}

	void Assert(bool condition, const char* conditionText, const char* file, int line, const char* message)
	{
		if (!condition)
		{
			std::cout << "Assert failed: " << conditionText << "\n";
			std::cout << file << ":" << line << "\n";
			std::cout << message << std::flush;
			DebugBreak();
			std::raise(SIGABRT);
		}
	}
}

#endif