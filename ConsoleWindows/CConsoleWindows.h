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
	unsigned char ForegroundColorIndex;
	unsigned char BackgroundColorIndex;
	wchar_t  CharacterCode;
} typedef ConsolePixel;
struct {
	unsigned char Index;
	ConsoleColor Color;
} typedef PaletteInfo;


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
		const ConsolePixel* buffer,
		const int width,
		const int height,
		const int left,
		const int top
	);
	void CopyBufferToScreenWinApi(
		const ConsolePixel* buffer,
		const int width,
		const int height,
		const int left,
		const int top
	);
	void CopyBufferToScreenVT100(
		const ConsolePixel* buffer,
		const int width,
		const int height,
		const int left,
		const int top
	);
	int SetScreenColors(
		const int numColors,
		const PaletteInfo* paletteInfo
	);
	int SetScreenColorsVT100(
		const int numColors,
		const PaletteInfo* paletteInfo
	);
	int SetScreenColorsWinApi(
		const int numColors,
		const PaletteInfo* paletteInfo
	);
	void ClearScreen(
		const ConsolePixel clearPixel,
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
	int _fontWidth;
	int _fontHeight;

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
		const ConsolePixel* buffer,
		const int width,
		const int height,
		const int left,
		const int top
	);
	CONSOLEWINDOWSDLL_API int SetScreenColors(
		CConsoleWindows* consoleWindow,
		const int numColors,
		const PaletteInfo* paletteInfo
	);
	CONSOLEWINDOWSDLL_API void ClearScreen(
		CConsoleWindows* consoleWindow,
		const ConsolePixel clearPixel,
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
