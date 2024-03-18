#pragma once
#include <format>

#define ANSII_ESCAPE "\033"
#define COLOR_FG(r,g,b) std::format(ANSII_ESCAPE "[38;2;{};{};{}m", r, g, b)
#define COLOR_BG(r,g,b) std::format(ANSII_ESCAPE "[48;2;{};{};{}m", r, g, b)
#define CURSOR_MOVE(x,y) std::format(ANSII_ESCAPE "[{};{}H", y, x)
