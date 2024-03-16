using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class ConsoleRasterizer
    {
        static ConsoleRasterizer()
        {
        }

        public static void CopyBufferToScreen(
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

        public static void InitalizeSurface(int width, int height, int fontWidth, int fontHeight)
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

        public static void ClearScreen(
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

        public static bool HandleWindowResize(out SMALL_RECT newWindowSize)
        {
            bool result = ConsoleWindows.HandleWindowResize(_consoleWindow, out newWindowSize);
            if (result)
            {
                WindowWidth = newWindowSize.Right - newWindowSize.Left + 1;
                WindowHeight = newWindowSize.Bottom - newWindowSize.Top + 1;
            }
            return result;
        }

        public static void UpdateConsoleTitle(float elapsedSeconds)
        {
            ConsoleWindows.UpdateConsoleTitle(_consoleWindow, elapsedSeconds);
        }

        public static void CreateConsoleWindow()
        {
            _consoleWindow = ConsoleWindows.CreateConsoleWindows();
        }

        public static int WindowWidth { get; set; }
        public static int WindowHeight { get; set; }
    }
}
