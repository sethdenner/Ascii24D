#include "pch.h"

#define CONSOLEWINDOWSDLL_EXPORTS
#include "CConsoleWindows.h"
#include "EscapeSequences.h"

#include <stdio.h>
#include <iostream>
#include <sstream>

// Forward declarations for objects required for maintaining thread saftey.
std::atomic<bool> CConsoleWindows::_isAtomActive;
std::mutex CConsoleWindows::_gameMutex;
std::condition_variable CConsoleWindows::_gameFinished;

CConsoleWindows::CConsoleWindows()
{
	_consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
	_consoleHandleIn = GetStdHandle(STD_INPUT_HANDLE);

	_consoleWindow = GetConsoleWindow();

}

int CConsoleWindows::InitializeConsole(
	int width,
	int height,
	int fontWidth,
	int fontHeight
) {
	_consoleHandle = CreateConsoleScreenBuffer(
		GENERIC_READ | GENERIC_WRITE,
		0,
		NULL,
		CONSOLE_TEXTMODE_BUFFER,
		NULL
	);
	SetConsoleActiveScreenBuffer(_consoleHandle);
	SetConsoleCP(CP_UTF8);

	if (_consoleHandle == INVALID_HANDLE_VALUE)
		return Error(L"Bad Handle");

	_screenWidth = width;
	_screenHeight = height;

	// Update 13/09/2017 - It seems that the console behaves differently on some
	// systems and I'm unsure why this is. It could be to do with windows
	// default settings, or screen resolutions, or system languages.
	// Unfortunately, MSDN does not offer much by way of useful information,
	// and so the resulting sequence is the reult of experiment that seems to
	// work in multiple cases.
	//
	// The problem seems to be that the SetConsoleXXX functions are somewhat
	// circular and fail depending on the state of the current console
	// properties, i.e. you can't set the buffer size until you set the screen
	// size, but you can't change the screen size until the buffer size is
	// correct. This coupled with a precise ordering of calls makes this
	// procedure seem a little mystical :-P. Thanks to wowLinh for helping - Jx9

	// Change console visual size to a minimum so ScreenBuffer can shrink
	// below the actual visual size
	_rectWindow = { 0, 0, 1, 1 };
	SetConsoleWindowInfo(_consoleHandle, TRUE, &_rectWindow);

	// Set the size of the screen buffer
	COORD coord = { (short)_screenWidth, (short)_screenHeight };
	if (!SetConsoleScreenBufferSize(_consoleHandle, coord))
		Error(L"SetConsoleScreenBufferSize");

	// Assign screen buffer to the console
	if (!SetConsoleActiveScreenBuffer(_consoleHandle))
		return Error(L"SetConsoleActiveScreenBuffer");

	// Set the font size now that the screen buffer has been assigned to the
	// console.
	CONSOLE_FONT_INFOEX cfi;
	cfi.cbSize = sizeof(cfi);
	cfi.nFont = 0;
	cfi.dwFontSize.X = fontWidth;
	cfi.dwFontSize.Y = fontHeight;
	cfi.FontFamily = FF_DONTCARE;
	cfi.FontWeight = FW_NORMAL;

	// Using Perfect DOS Code 437 font. TODO: Make this configurable.
	wcscpy_s(cfi.FaceName, L"Perfect DOS VGA 437");
	if (!SetCurrentConsoleFontEx(_consoleHandle, false, &cfi))
		return Error(L"SetCurrentConsoleFontEx");

	// Get screen buffer info and check the maximum allowed window size. Return
	// error if exceeded, so user knows their dimensions/fontsize are too large
	CONSOLE_SCREEN_BUFFER_INFO csbi;
	if (!GetConsoleScreenBufferInfo(_consoleHandle, &csbi))
		return Error(L"GetConsoleScreenBufferInfo");
	if (_screenHeight > csbi.dwMaximumWindowSize.Y)
		return Error(L"Screen Height / Font Height Too Big");
	if (_screenWidth > csbi.dwMaximumWindowSize.X)
		return Error(L"Screen Width / Font Width Too Big");

	// Set Physical Console Window Size
	_rectWindow = { 0, 0, (short)_screenWidth - 1, (short)_screenHeight - 1 };
	if (!SetConsoleWindowInfo(_consoleHandle, TRUE, &_rectWindow))
		return Error(L"SetConsoleWindowInfo");

	// Set flags to hide cursor
	CONSOLE_CURSOR_INFO cursorInfo;
	GetConsoleCursorInfo(_consoleHandle, &cursorInfo);
	cursorInfo.bVisible = false;
	cursorInfo.dwSize = 1;
	SetConsoleCursorInfo(_consoleHandle, &cursorInfo);

	// Hide scroll bars.
	ShowScrollBar(GetConsoleWindow(), SB_BOTH, FALSE);

	// Set window fully opaque.
	SetLayeredWindowAttributes(GetConsoleWindow(), NULL, 255, LWA_ALPHA);

	// Set flags to allow mouse input		
	if (!SetConsoleMode(
		_consoleHandleIn,
		ENABLE_EXTENDED_FLAGS |
		ENABLE_WINDOW_INPUT |
		ENABLE_MOUSE_INPUT
	)) return Error(L"SetConsoleMode");

	// Allocate memory for screen buffer
	if (NULL != _screenBuffer)
	{
		delete _screenBuffer;
	}
	_screenBuffer = new CHAR_INFO[_screenWidth * _screenHeight];
	memset(_screenBuffer, 0, sizeof(CHAR_INFO) * _screenWidth * _screenHeight);

	SetConsoleCtrlHandler((PHANDLER_ROUTINE)OnClose, TRUE);

	return 1;
}

int CConsoleWindows::Error(const wchar_t* msg)
{
	wchar_t buf[256];
	FormatMessage(
		FORMAT_MESSAGE_FROM_SYSTEM,
		NULL,
		GetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		buf,
		256,
		NULL
	);
	SetConsoleActiveScreenBuffer(_originalConsole);
	wprintf(L"ERROR: %s\n\t%s\n", msg, buf);
	return 0;
}

BOOL CConsoleWindows::OnClose(DWORD evt)
{
	// Note this gets called in a seperate OS thread, so it must only exit when
	// the game has finished cleaning up, or else the process will be killed
	// before OnUserDestroy() has finished.
	if (evt == CTRL_CLOSE_EVENT)
	{
		_isAtomActive = false;

		// Wait for thread to be exited
		std::unique_lock<std::mutex> ul(_gameMutex);
		_gameFinished.wait(ul);
	}
	return true;
}
void CConsoleWindows::CopyBufferToScreen(
	const ConsolePixel* buffer,
	const int width,
	const int height,
	const int left,
	const int top
) {
	CopyBufferToScreenWinApi(
		buffer,
		width,
		height,
		left,
		top
	);
}
void CConsoleWindows::CopyBufferToScreenVT100(
	const ConsolePixel* buffer,
	const int width,
	const int height,
	const int left,
	const int top
) {
	std::wostringstream output;
	int bufferLength = width * height;
	output << CURSOR_MOVE(left, top);
	ConsolePixel previousPixel = {};
	for (int i = 0; i < bufferLength; ++i)
	{
		const ConsolePixel pixel = buffer[i];
		if (0 == pixel.CharacterCode) {
			continue;
		}
		if (
			pixel.ForegroundColorIndex != previousPixel.ForegroundColorIndex
		) {
			output << COLOR_FG_I(pixel.ForegroundColorIndex);
		}
		if (
			pixel.BackgroundColorIndex != previousPixel.BackgroundColorIndex
		) {
			output << COLOR_BG_I(pixel.BackgroundColorIndex);
		}
		output << (char)pixel.CharacterCode;
		previousPixel = pixel;
	}

	std::wstring outputString = output.str();
	DWORD charsWritten = 0;
	WriteConsole(
		_consoleHandle,
		outputString.c_str(),
		outputString.length(),
		&charsWritten,
		NULL
	);
}
void CConsoleWindows::CopyBufferToScreenWinApi(
	const ConsolePixel* buffer,
	const int width,
	const int height,
	const int left,
	const int top
) {
	std::wostringstream output;
	int bufferLength = width * height;
	CHAR_INFO* charInfo = new CHAR_INFO[bufferLength];
	for (int i = 0; i < bufferLength; ++i) {
		ConsolePixel pixel = buffer[i];
		charInfo[i] = {
			pixel.CharacterCode,
			static_cast<WORD>(
				pixel.ForegroundColorIndex |
				(pixel.BackgroundColorIndex << 4)
			)
		};
	}

	COORD bufferSize = {width, height};
	COORD bufferCoord = {};
	SMALL_RECT writeRegion = {
		left, // Left
		top, // Top
		width, // Right
		height // Bottom
	};

	WriteConsoleOutput(
		_consoleHandle,
		charInfo,
		bufferSize,
		bufferCoord,
		&writeRegion
	);

	delete[] charInfo;
}
int CConsoleWindows::SetScreenColors(
	const int numColors,
	const PaletteInfo* paletteInfo
) {
	return SetScreenColorsWinApi(
		numColors,
		paletteInfo
	);
}

int CConsoleWindows::SetScreenColorsVT100(
	const int numColors,
	const PaletteInfo* paletteInfo
) {
	int result = 0; // OK
	std::wostringstream output;
	for (int i = 0; i < numColors; ++i) {
		PaletteInfo info = paletteInfo[i];
		output << SET_PALETTE_COLOR(info.Index, info.Color.R, info.Color.G, info.Color.B);
	}

	std::wstring outputString = output.str();
	DWORD charsWritten = 0;
	result += 1 ^ WriteConsole(
		_consoleHandle,
		outputString.c_str(),
		outputString.length(),
		&charsWritten,
		NULL
	);
	return result;
}

int CConsoleWindows::SetScreenColorsWinApi(
	const int numColors,
	const PaletteInfo* paletteInfo
) {
	int result = 0; // OK
	CONSOLE_SCREEN_BUFFER_INFOEX screenBufferInfo = {};
	screenBufferInfo.cbSize = sizeof(CONSOLE_SCREEN_BUFFER_INFOEX);
	result += 1 ^ GetConsoleScreenBufferInfoEx(
		_consoleHandle,
		&screenBufferInfo
	);
	screenBufferInfo.wAttributes = (
		FOREGROUND_RED |
		FOREGROUND_GREEN |
		FOREGROUND_BLUE
	);

	if (result) { return result; }

	for (int i = 0; i < numColors; ++i) {
		PaletteInfo pInfo = paletteInfo[i];
		screenBufferInfo.ColorTable[pInfo.Index] = RGB(
			pInfo.Color.R,
			pInfo.Color.G,
			pInfo.Color.B
		);
	}

	result += 1 ^ SetConsoleScreenBufferInfoEx(
		_consoleHandle,
		&screenBufferInfo
	);

	if (result) { return result; }

	return result;
}

void CConsoleWindows::ClearScreen(
	const ConsolePixel clearPixel,
	const short width,
	const short height,
	const short left,
	const short top
) {
	COORD bufferDimensions = { width, height };
	bufferDimensions.X = width;
	bufferDimensions.Y = height;
	COORD bufferCoord = {};
	SMALL_RECT writeRegion = { top, left, left + width, top + height };

	const int bufferSize = width * height;
	ConsolePixel* buffer = new ConsolePixel[bufferSize]();
	memset(buffer, 0, sizeof(ConsolePixel) * bufferSize);

	for (int i = 0; i < bufferSize; ++i)
	{
		buffer[i] = clearPixel;
	}

	CopyBufferToScreen(buffer, width, height, left, top);
}

bool CConsoleWindows::HandleWindowResize(SMALL_RECT &newWindowSize)
{
	CONSOLE_SCREEN_BUFFER_INFO screenBufferInfo = {};
	GetConsoleScreenBufferInfo(_consoleHandle, &screenBufferInfo);

	bool windowResized = false;

	if (
		screenBufferInfo.srWindow.Right != _rectWindow.Right ||
		screenBufferInfo.srWindow.Bottom != _rectWindow.Bottom ||
		screenBufferInfo.srWindow.Left != _rectWindow.Left ||
		screenBufferInfo.srWindow.Top != _rectWindow.Top
		)
	{
		InitializeConsole(
			screenBufferInfo.srWindow.Right - screenBufferInfo.srWindow.Left,
			screenBufferInfo.srWindow.Bottom - screenBufferInfo.srWindow.Top,
			12,
			12
		);

		newWindowSize = {};
		newWindowSize = _rectWindow;
		windowResized = true;
	}

	return windowResized;;
}

bool CConsoleWindows::GetWindowSize(SMALL_RECT& windowSize)
{
	CONSOLE_SCREEN_BUFFER_INFO csbi;
	if (!GetConsoleScreenBufferInfo(_consoleHandle, &csbi))
		return Error(L"GetConsoleScreenBufferInfo");

	windowSize = csbi.srWindow;
	return true;

}

void CConsoleWindows::UpdateConsoleTitle(const float elapsedTime)
{
	// Update Title & Present Screen Buffer
	wchar_t s[256];
	swprintf_s(
		s,
		256,
		L"Ascii24D Console Game - %hs - FPS: %3.2f",
		"Test Game",
		1.0f / elapsedTime
	);
	SetConsoleTitle(s);

}

CConsoleWindows* CreateConsoleWindows()
{
    return new CConsoleWindows();
}

void DestroyConsoleWindows(CConsoleWindows* consoleWindow)
{
    delete consoleWindow;
}

int InitializeConsole(
	CConsoleWindows* consoleWindow,
	int width,
	int height,
	int fontWidth,
	int fontHeight
) {
	return consoleWindow->InitializeConsole(
		width,
		height,
		fontWidth,
		fontHeight
	);
}

void CopyBufferToScreen(
	CConsoleWindows* consoleWindow,
	const ConsolePixel* buffer,
	const int width,
	const int height,
	const int left,
	const int top
) {
	consoleWindow->CopyBufferToScreen(
		buffer,
		width,
		height,
		left,
		top
	);
}

int SetScreenColors(
	CConsoleWindows* consoleWindow,
	const int numColors,
	const PaletteInfo* paletteInfo
) {
	return consoleWindow->SetScreenColors(
		numColors,
		paletteInfo
	);
}

bool HandleWindowResize(
	CConsoleWindows* consoleWindow,
	SMALL_RECT &newWindowSize
) {
	return consoleWindow->HandleWindowResize(newWindowSize);
}

 void UpdateConsoleTitle(
	 CConsoleWindows* consoleWindow,
	 const float elapsedTime
) {
	 consoleWindow->UpdateConsoleTitle(elapsedTime);
}

void ClearScreen(
	CConsoleWindows* consoleWindow,
	const ConsolePixel clearPixel,
	const short width,
	const short height,
	const short left,
	const short top
) {
	consoleWindow->ClearScreen(
		clearPixel,
		width,
		height,
		left,
		top
	);
}

bool GetWindowSize(CConsoleWindows* consoleWindow, SMALL_RECT &windowSize)
{
	return consoleWindow->GetWindowSize(windowSize);
}
