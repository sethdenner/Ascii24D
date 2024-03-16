using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal static class ConsoleRasterizer
    {
        static ConsoleRasterizer()
        {
        }

        internal static void CopyBufferToScreen(
            CHAR_INFO[] buffer,
            int width,
            int height,
            int left = 0,
            int top = 0
        )
        {
            ConsoleWindows.CopyBufferToScreen(
                _consoleWindow,
                buffer,
                width,
                height,
                left,
                top
            );
        }

        private static IntPtr _consoleWindow;

        internal static void InitalizeSurface(int width, int height, int fontWidth, int fontHeight)
        {
            ConsoleWindows.InitializeConsole(
                _consoleWindow,
                width,
                height,
                fontWidth,
                fontHeight
            );
            ConsoleRasterizer.WindowWidth = width;
            ConsoleRasterizer.WindowHeight = height;
        }

        internal static void ClearScreen(
            ushort attributes,
            int width,
            int height,
            int left = 0,
            int top = 0
        )
        {
            ConsoleWindows.ClearScreen(
                _consoleWindow,
                attributes,
                width,
                height,
                left,
                top
            );
        }

        internal static bool HandleWindowResize(out SMALL_RECT newWindowSize)
        {
            bool result = ConsoleWindows.HandleWindowResize(_consoleWindow, out newWindowSize);
            if (result)
            {
                WindowWidth = newWindowSize.Right - newWindowSize.Left + 1;
                WindowHeight = newWindowSize.Bottom - newWindowSize.Top + 1;
            }
            return result;
        }

        internal static void UpdateConsoleTitle(float elapsedSeconds)
        {
            ConsoleWindows.UpdateConsoleTitle(_consoleWindow, elapsedSeconds);
        }

        internal static void CreateConsoleWindow()
        {
            _consoleWindow = ConsoleWindows.CreateConsoleWindows();
        }

        internal static int WindowWidth { get; set; }
        internal static int WindowHeight { get; set; }
    }
}
