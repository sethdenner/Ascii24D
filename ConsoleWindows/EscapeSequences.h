#pragma once
#include <format>
#include <string>

#define ANSII_ESCAPE L"\033"
#define COLOR_FG(r,g,b) ANSII_ESCAPE << L"[38;2;" << (int)r << L";" << (int)g << L";" << (int)b << L"m"
#define COLOR_BG(r,g,b) ANSII_ESCAPE << L"[48;2;" << (int)r << L";" << (int)g << L";" << (int)b << L"m"
#define CURSOR_MOVE(x,y) ANSII_ESCAPE << L"[" << (int)y << L";" << (int)x << L"H"
