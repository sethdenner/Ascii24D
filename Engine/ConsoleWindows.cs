using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Engine
{
    public enum CHAR_INFO_ATTRIBUTE
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
        COMMON_LVB_GRID_HORIZONTAL = 0x0400,
        COMMON_LVB_GRID_LVERTICAL = 0x0800,
        COMMON_LVB_GRID_RVERTICAL = 0x1000,
        COMMON_LVB_REVERSE_VIDEO = 0x4000,
        COMMON_LVB_UNDERSCORE = 0x8000
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct CHAR_INFO
    {
        public char Char;
        public ushort Attributes; // Combination of CHAR_INFO_ATTRIBUTE enum flags.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SMALL_RECT
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
    }

    internal static class ConsoleWindows
    {
        [DllImport("ConsoleWindows.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateConsoleWindows();
        [DllImport("ConsoleWindows.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyConsoleWindow(IntPtr consoleWindow);
        [DllImport("ConsoleWindows.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitializeConsole(IntPtr consoleWindow, int width, int height, int fontWidth, int fontHeight);

        [DllImport("ConsoleWindows.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CopyBufferToScreen(IntPtr consoleWindow, CHAR_INFO[] buffer, int width, int height, int left,int top);
        [DllImport("ConsoleWindows.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ClearScreen(IntPtr consoleWindow, ushort attributes, int width, int height, int left, int top);
        [DllImport("ConsoleWindows.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool HandleWindowResize(IntPtr consoleWindow, out SMALL_RECT newWindowSize);
        [DllImport("ConsoleWindows.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetWindowSize(IntPtr consoleWindow, out SMALL_RECT windowSize);
        [DllImport("ConsoleWindows.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void UpdateConsoleTitle(IntPtr consoleWindow, float elapsedTime);
    }
}
