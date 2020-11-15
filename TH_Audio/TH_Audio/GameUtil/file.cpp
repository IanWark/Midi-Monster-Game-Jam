#include "file.h"

#include "debugConsole.h"
//#include "json11/json11.hpp"
#include "memory.h"

#include <fstream>

#if _WIN32
#include <Windows.h>
#else
#include <sys/stat.h>
#endif

namespace Utils
{
	bool DoesFileExist(const char* path)
	{
		DWORD attribs = GetFileAttributes(path);
		return (attribs != INVALID_FILE_ATTRIBUTES && !(attribs &= FILE_ATTRIBUTE_DIRECTORY));
	}

	int GetFileSize(const char* path)
	{
#if _WIN32
		WIN32_FILE_ATTRIBUTE_DATA data;
		GetFileAttributesEx(path, GetFileExInfoStandard, &data);
		// Todo: this is kinda wrong:
		ASSERT(data.nFileSizeHigh == 0, "Failed to get file size.");
		return data.nFileSizeLow;
#else
		struct stat statbuf;
		if (stat(path, &statbuf) == -1) {
			ASSERT(false, "Failed to get file size.");
  		}
  
 		return statbuf.st_size;
#endif
		return 0;
	}

	void GetTextFromFile(const char* aPath, char *rText, int* rLength)
	{
		int maxLength = *rLength;
		*rLength = 0;

		std::fstream file(aPath, std::fstream::in);
		ASSERT(file.is_open(), "Failed to open file.");

		bool done = false;
		int index = 0;

		while (!done)
		{
			char input = file.get();
			if (!file.eof())
			{
				rText[index] = input;
				(*rLength)++;

				if (*rLength == maxLength)
				{
					done = true;
				}
				else
				{
					index++;
				}
			}
			else
			{
				done = true;
			}
		}

		rText[index] = 0; // Terminate the string with a null

		file.close();
	}

	//json11::Json* CreateJsonFromFile(const char* aPath)
	//{
	//	int fileSize = GetFileSize(aPath);
	//	int resultLength = fileSize + 1;
	//	char* buffer = CREATE_ARRAY(char,fileSize + 1);

	//	GetTextFromFile(aPath, buffer, &resultLength);

	//	std::string err;
	//	json11::Json* out = CREATE(json11::Json);
	//	*out = json11::Json::parse(buffer, err);

	//	if (err.length() > 0)
	//	{
	//		// Should assert here...?
	//		std::string errorHead = "JSON ERROR - ";
	//		errorHead += aPath;
	//		DEBUG_LOG_COLOR(Utils::Red, errorHead);
	//		DEBUG_LOG_COLOR(Utils::Red, err);
	//	}

	//	DESTROY(buffer);
	//	return out;
	//}

	std::vector<std::string> FindFiles(const char* path, const char* ext)
	{
		std::vector<std::string> result;
		WIN32_FIND_DATA findData;

		char fullPath[2048];
		GetCurrentDirectory(2048, fullPath);

		snprintf(fullPath, 2048, "%s/%s*.%s", fullPath, path, ext);

//#if _DEBUG
//		char debugText[1024];
//		snprintf(debugText, 1024, "Searching for .%s files in %s", ext, path);
//		DEBUG_LOG(debugText);
//#endif
//
		HANDLE handle = FindFirstFile(fullPath, &findData);

		if (handle != INVALID_HANDLE_VALUE)
		{
			do
			{
				result.push_back(findData.cFileName);
			} while (FindNextFile(handle, &findData));
		}

		return result;
	}

	std::vector<std::string> FindFolders(const char* path)
	{
		std::vector<std::string> result;
		WIN32_FIND_DATA findData;

		char fullPath[2048];
		GetCurrentDirectory(2048, fullPath);

		snprintf(fullPath, 2048, "%s/%s*", fullPath, path);

		HANDLE handle = FindFirstFile(fullPath, &findData);

		if (handle != INVALID_HANDLE_VALUE)
		{
			do
			{
				if (findData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY && findData.cFileName[0] != '.')
				{
					result.push_back(findData.cFileName);
				}
			} while (FindNextFile(handle, &findData));
		}

		return result;
	}

	void WriteTextToFile(const char* aPath, const char *rText)
	{
		std::fstream file(aPath, std::fstream::out);
		ASSERT(file.is_open(), "Failed to open file.");

		const char* current = rText;
		while (*current != 0)
		{
			file.put(*current);
			current++;
		}

		file.close();
	}
}