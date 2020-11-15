#ifndef FILE_H
#define FILE_H

#include <string>
#include <vector>

namespace json11
{
	class Json;
}

namespace Utils
{
	bool DoesFileExist(const char* path);
	int GetFileSize(const char* path);

	void GetTextFromFile(const char* aPath, char *rText, int* rLength);

	void WriteTextToFile(const char* aPath, const char *rText);

	json11::Json* CreateJsonFromFile(const char* aPath);

	std::vector<std::string> FindFiles(const char* path, const char* ext);
	std::vector<std::string> FindFolders(const char* path);
}

#endif