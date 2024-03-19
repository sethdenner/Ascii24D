#pragma once

#include <thread>
#include <atomic>
#include <mutex>

#define WIN32_LEAN_AND_MEAN
#include <Windows.h>


// The following ifdef block is the standard way of creating macros which make
// exporting from a DLL simpler. All files within this DLL are compiled with the
// PINVOKELIB_EXPORTS symbol defined on the command line. this symbol should not
// be defined on any project that uses this DLL. This way any other project
// whose source files include this file see PINVOKELIB_API functions as being
// imported from a DLL, wheras this DLL sees symbols defined with this macro as
// being exported.
#ifdef CONSOLEWINDOWSDLL_EXPORTS
#define CONSOLEWINDOWSDLL_API __declspec(dllexport)
#else
#define CONSOLEWINDOWSDLL_API __declspec(dllimport)
#endif

struct
{
	unsigned char R;
	unsigned char G;
	unsigned char B;
} typedef ConsoleColor;
struct
{
	ConsoleColor ForegroundColor;
	ConsoleColor BackgroundColor;
	unsigned char CharacterCode;
} typedef ConsolePixel;

enum COLOUR
{
	FG_BLACK = 0x0000,
	FG_DARK_BLUE = 0x0001,
	FG_DARK_GREEN = 0x0002,
	FG_DARK_CYAN = 0x0003,
	FG_DARK_RED = 0x0004,
	FG_DARK_MAGENTA = 0x0005,
	FG_DARK_YELLOW = 0x0006,
	FG_GREY = 0x0007,
	FG_DARK_GREY = 0x0008,
	FG_BLUE = 0x0009,
	FG_GREEN = 0x000A,
	FG_CYAN = 0x000B,
	FG_RED = 0x000C,
	FG_MAGENTA = 0x000D,
	FG_YELLOW = 0x000E,
	FG_WHITE = 0x000F,
	BG_BLACK = 0x0000,
	BG_DARK_BLUE = 0x0010,
	BG_DARK_GREEN = 0x0020,
	BG_DARK_CYAN = 0x0030,
	BG_DARK_RED = 0x0040,
	BG_DARK_MAGENTA = 0x0050,
	BG_DARK_YELLOW = 0x0060,
	BG_GREY = 0x0070,
	BG_DARK_GREY = 0x0080,
	BG_BLUE = 0x0090,
	BG_GREEN = 0x00A0,
	BG_CYAN = 0x00B0,
	BG_RED = 0x00C0,
	BG_MAGENTA = 0x00D0,
	BG_YELLOW = 0x00E0,
	BG_WHITE = 0x00F0,
};

class CONSOLEWINDOWSDLL_API CConsoleWindows
{
public:
    CConsoleWindows();
    int InitializeConsole(
		int width,
		int height,
		int fontWidth,
		int fontHeight
	);
    void CopyBufferToScreen(
		const CHAR_INFO* buffer,
		const short width,
		const short height,
		const short left,
		const short top
	);
	void CopyBufferToScreenVT(
		const ConsolePixel* buffer,
		const int width,
		const int height,
		const int left,
		const int top
	);
	void ClearScreen(
		const unsigned short attributes,
		const short width,
		const short height,
		const short left,
		const short top
	);
	bool HandleWindowResize(SMALL_RECT& newWindowSize);
	bool GetWindowSize(SMALL_RECT& windowSize);
	void UpdateConsoleTitle(const float elapsedTime);

protected:
	int Error(const wchar_t* msg);

    static BOOL OnClose(DWORD evt);

private:
    HANDLE _consoleHandle;
    HANDLE _consoleHandleIn;
    HANDLE _originalConsole;

	HWND _consoleWindow;

    SMALL_RECT _rectWindow;
    CHAR_INFO* _screenBuffer;
    int _screenWidth;
    int _screenHeight;

	static std::atomic<bool> _isAtomActive;
	static std::mutex _gameMutex;
	static std::condition_variable _gameFinished;
};

#ifdef __cplusplus
extern "C"
{
#endif

    CONSOLEWINDOWSDLL_API CConsoleWindows* CreateConsoleWindows();
	CONSOLEWINDOWSDLL_API void DestroyConsoleWindows(
		CConsoleWindows* consoleWindow
	);
	CONSOLEWINDOWSDLL_API int InitializeConsole(
		CConsoleWindows* consoleWindow,
		int width,
		int height,
		int fontWidth,
		int fontHeight
	);
	CONSOLEWINDOWSDLL_API void CopyBufferToScreen(
		CConsoleWindows* consoleWindow,
		const CHAR_INFO* buffer,
		const int width,
		const int height,
		const int left,
		const int top
	);
	CONSOLEWINDOWSDLL_API void CopyBufferToScreenVT(
		CConsoleWindows* consoleWindow,
		const ConsolePixel* buffer,
		const int width,
		const int height,
		const int left,
		const int top
	);
	CONSOLEWINDOWSDLL_API void ClearScreen(
		CConsoleWindows* consoleWindow,
		const WORD attributes,
		const short width,
		const short height,
		const short left,
		const short top
	);
	CONSOLEWINDOWSDLL_API bool HandleWindowResize(
		CConsoleWindows* consoleWindow,
		SMALL_RECT& newWindowSize
	);
	CONSOLEWINDOWSDLL_API bool GetWindowSize(
		CConsoleWindows* consoleWindow,
		SMALL_RECT& windowSize
	);
	CONSOLEWINDOWSDLL_API void UpdateConsoleTitle(
		CConsoleWindows* consoleWindow,
		const float elapsedTime
	);

#ifdef __cplusplus
}
#endif
