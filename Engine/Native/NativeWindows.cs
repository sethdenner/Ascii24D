using System.Runtime.InteropServices;

namespace Engine.Native
{
    /// <summary>
    /// 
    /// </summary>
    public enum CHAR_INFO_ATTRIBUTE : ushort
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
    /// <summary>
    /// <c>CHAR_INFO</c> struct holds information for rendering a single console
    /// character including color and ohter style attributes as well as the font
    /// glyph to be rendered. This struct implements the C++ Windows API struct
    /// <c>CHAR_INFO</c> and can be used to marshal objects of that type from
    /// C++ code.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct CHAR_INFO
    {
        /// <summary>
        /// <c>Char</c> is the value representing the font glyph to render.
        /// </summary>
        public byte Char;
        /// <summary>
        /// <c>Attributes</c> is a combination of CHAR_INFO_ATTRIBUTE enum
        /// flags.
        /// </summary>
        public ushort Attributes;
    }
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SMALL_RECT
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
    }
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ConsoleColor
    {
        public byte R;
        public byte G;
        public byte B;
    }
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ConsolePixel
    {
        public ConsoleColor ForegroundColor;
        public ConsoleColor BackgroundColor;
        public byte CharacterCode;
    }
    /// <summary>
    /// 
    /// </summary>
    internal static class NativeWindows
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(
            "ConsoleWindows.dll",
            CallingConvention = CallingConvention.Cdecl
        )]
        public static extern nint CreateConsoleWindows();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consoleWindow"></param>
        [DllImport(
            "ConsoleWindows.dll",
            CallingConvention = CallingConvention.Cdecl
        )]
        public static extern void DestroyConsoleWindow(nint consoleWindow);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consoleWindow"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fontWidth"></param>
        /// <param name="fontHeight"></param>
        /// <returns></returns>
        [DllImport(
            "ConsoleWindows.dll",
            CallingConvention = CallingConvention.Cdecl
        )]
        public static extern int InitializeConsole(
            nint consoleWindow,
            int width,
            int height,
            int fontWidth,
            int fontHeight
        );
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consoleWindow"></param>
        /// <param name="buffer"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        [DllImport(
            "ConsoleWindows.dll",
            CallingConvention = CallingConvention.Cdecl
        )]
        public static extern void CopyBufferToScreen(
            nint consoleWindow,
            CHAR_INFO[] buffer,
            int width,
            int height,
            int left,
            int top
        );
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consoleWindow"></param>
        /// <param name="buffer"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        [DllImport(
            "ConsoleWindows.dll",
            CallingConvention = CallingConvention.Cdecl
        )]
        public static extern void CopyBufferToScreenVT(
            nint consoleWindow,
            ConsolePixel[] buffer,
            int width,
            int height,
            int left,
            int top
        );
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consoleWindow"></param>
        /// <param name="attributes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        [DllImport(
            "ConsoleWindows.dll",
            CallingConvention = CallingConvention.Cdecl
        )]
        public static extern void ClearScreen(
            nint consoleWindow,
            ushort attributes,
            int width,
            int height,
            int left,
            int top
        );
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consoleWindow"></param>
        /// <param name="newWindowSize"></param>
        /// <returns></returns>
        [DllImport(
            "ConsoleWindows.dll",
            CallingConvention = CallingConvention.Cdecl
        )]
        public static extern bool HandleWindowResize(
            nint consoleWindow,
            out SMALL_RECT newWindowSize
        );
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consoleWindow"></param>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        [DllImport(
            "ConsoleWindows.dll",
            CallingConvention = CallingConvention.Cdecl
        )]
        public static extern bool GetWindowSize(
            nint consoleWindow,
            out SMALL_RECT windowSize
        );
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consoleWindow"></param>
        /// <param name="elapsedTime"></param>
        [DllImport(
            "ConsoleWindows.dll",
            CallingConvention = CallingConvention.Cdecl
        )]
        public static extern void UpdateConsoleTitle(
            nint consoleWindow,
            float elapsedTime
        );
    }
}
