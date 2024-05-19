using System.Diagnostics;
using SharpDX.DirectInput;
using Engine.Input;
using Engine.Characters;
using Engine.Characters.UI;
using Engine.Render;
using Engine.Native;
using Engine.Core;
using System.Numerics;

namespace DirectInputDebugDemo
{
    /// <summary>
    /// Program entry point class.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public static Sprite CreateFramebuffer(
            int width,
            int height,
            int left,
            int top
        )
        {
            return new Sprite(
                width,
                height,
                left,
                top
            );
        }
        public static Sprite ResizeWindow(SMALL_RECT newWindowSize)
        {
            Native.InitalizeSurface(
               newWindowSize.Right - newWindowSize.Left,
               newWindowSize.Bottom - newWindowSize.Top,
               12,
               12
           );
            return CreateFramebuffer(
               newWindowSize.Right - newWindowSize.Left,
               newWindowSize.Bottom - newWindowSize.Top,
               newWindowSize.Left,
               newWindowSize.Top
           );
        }
        static int Main(string[] args) {
            DirectInput sharpDXDirectInput = new DirectInput();
            SharpDXDirectInputWrapper sharpDxDirectInputWrapper = (
                new SharpDXDirectInputWrapper(sharpDXDirectInput)
            );
            Input input = new Input(sharpDxDirectInputWrapper);
            input.EnumerateDevices();

            int fontWidth = 12;
            int fontHeight = 12;

            Native.CreateConsoleWindow();
            Native.InitalizeSurface(
                80,
                50,
                fontWidth,
                fontHeight
            );

            Console.Clear();

            PaletteInfo[] palette = [
                new PaletteInfo() { // Black
                    Color = new Engine.Native.ConsoleColor() {
                        R = 0,
                        G = 0,
                        B = 0
                    },
                    Index = 0
                },
                new PaletteInfo() { // White
                    Color = new Engine.Native.ConsoleColor() {
                        R = 255,
                        G = 255,
                        B = 255
                    },
                    Index = 1
                },
                new PaletteInfo() { // Red
                    Color = new Engine.Native.ConsoleColor() {
                        R = 255,
                        G = 0,
                        B = 0
                    },
                    Index = 2
                },
                new PaletteInfo() { // Light Gray
                    Color = new Engine.Native.ConsoleColor() {
                        R = 76,
                        G = 76,
                        B = 76
                    },
                    Index = 3
                },
                new PaletteInfo() { // Dark Gray
                    Color = new Engine.Native.ConsoleColor() {
                        R = 25,
                        G = 25,
                        B = 25
                    },
                    Index = 4
                }
            ];

            Native.SetScreenColors(5, palette);

            Sprite framebuffer = CreateFramebuffer(
                Native.WindowWidth,
                Native.WindowHeight,
                0,
                0
            );

            CharacterFrameCounter fpsCounter = new CharacterFrameCounter(
                0,
                3
            );
            UIWindowTextDebugInput inputDebugWindow = (
                new UIWindowTextDebugInput(input)
            );
            inputDebugWindow.Position = new Vector3(
                inputDebugWindow.Position.X,
                inputDebugWindow.Position.Y,
                0
            );
            Map demoMap = new Map();
            demoMap.AddCharacter(fpsCounter);
            demoMap.AddCharacter(inputDebugWindow);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            long frameStartTime = stopwatch.ElapsedTicks;

            do
            {
                long elapsedTime = stopwatch.ElapsedTicks - frameStartTime;
                frameStartTime += elapsedTime;

                float elapsedSeconds = (
                    (float)elapsedTime / (float)Stopwatch.Frequency
                );


                SMALL_RECT newWindowSize;
                if (Native.HandleWindowResize(out newWindowSize))
                {
                    framebuffer = ResizeWindow(newWindowSize);
                }

                try
                {
                    framebuffer.Fill(new ConsolePixel() {
                        ForegroundColorIndex = 2,
                        BackgroundColorIndex = 2,
                        CharacterCode = (byte)' '
                    });
                }
                catch (System.IndexOutOfRangeException)
                {
                    framebuffer = ResizeWindow(newWindowSize);
                }

                input.Update();
                Native.UpdateConsoleTitle(elapsedSeconds);

                fpsCounter.Update(elapsedSeconds);
                fpsCounter.Position = new Vector3(
                    framebuffer.Width - fpsCounter.FpsString.Length,
                    0,
                    0
                );
                inputDebugWindow.Update(elapsedSeconds);

                demoMap.Render(framebuffer);

                try
                {
                    Native.CopyBufferToScreen(
                        framebuffer.BufferPixels,
                        framebuffer.Width,
                        framebuffer.Height,
                        0,
                        0
                    );
                }
                catch (System.IndexOutOfRangeException)
                {
                    framebuffer = ResizeWindow(newWindowSize);
                }
            } while (true);
        }
    }
}
