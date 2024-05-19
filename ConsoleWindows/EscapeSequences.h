#pragma once
#include <format>
#include <string>

#define ANSII_ESCAPE L"\033"
#define ANSII_STRING_TERMINATOR ANSII_ESCAPE << L"\\"
#define COLOR_FG(r,g,b) ANSII_ESCAPE << L"[38;2;" << (int)r << L";" << (int)g << L";" << (int)b << L"m"
#define COLOR_BG(r,g,b) ANSII_ESCAPE << L"[48;2;" << (int)r << L";" << (int)g << L";" << (int)b << L"m"
#define SET_PALETTE_COLOR(i,r,g,b) ANSII_ESCAPE << L"]4;" << (int)i << L";rgb:" << (int)r << L"/" << (int)g << L"/" << (int)b << ANSII_STRING_TERMINATOR
#define CURSOR_MOVE(x,y) ANSII_ESCAPE << L"[" << (int)y << L";" << (int)x << L"H"
#define HIDE_CURSOR ANSII_ESCAPE << L"[?25l"
#define SHOW_CURSOR ANSII_ESCAPE << L"[?25h"
#define USE_ALT_SCREEN_BUFER ANSII_ESCAPE << L"[?1049h"
#define USE_MAIN_SCREEN_BUFFER ANSII_ESCAPE << L"[?1049h"
#define RESET_TERMINAL ANSII_ESCAPE << L"[!p"
#define COLOR_FG_I(i) ANSII_ESCAPE << L"[38;5;" << (int)i << L"m"
#define COLOR_BG_I(i) ANSII_ESCAPE << L"[48;5;" << (int)i << L"m"
