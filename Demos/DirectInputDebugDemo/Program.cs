using System.Diagnostics;
using SharpDX.DirectInput;
using Engine;
using Engine.Input;
using Engine.Characters;
using Engine.Characters.UI;

namespace DirectInputDebugDemo
{
    internal class Program
    {
        private static FrameBuffer CreateFrameBuffer(int width, int height)
        {
            FrameBuffer frameBuffer = new FrameBuffer(
                width,
                height
            );

            return frameBuffer;
        }

        static int Main(string[] args)
        {
            DirectInput sharpDXDirectInput = new DirectInput();
            SharpDXDirectInputWrapper sharpDxDirectInputWrapper = new SharpDXDirectInputWrapper(sharpDXDirectInput);
            Input input = new Input(sharpDxDirectInputWrapper);
            input.EnumerateDevices();

            int fontWidth = 12;
            int fontHeight = 12;

            // These values become unreliable once we start
            // changing the console window with the native 
            // windows api. Initialize with them here but we
            // need to keep track of these values ourselves.
            ConsoleRasterizer.CreateConsoleWindow();
            ConsoleRasterizer.InitalizeSurface(
                80,
                50,
                fontWidth,
                fontHeight
            );

            Console.Clear();

            FrameBuffer frameBuffer = CreateFrameBuffer(
                ConsoleRasterizer.WindowWidth,
                ConsoleRasterizer.WindowHeight
            );


            CharacterFrameCounter fpsCounter = new CharacterFrameCounter();
            UIWindowTextDebugInput inputDebugWindow = new UIWindowTextDebugInput(input);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            long frameStartTime = stopwatch.ElapsedTicks;

            do
            {
                long elapsedTime = stopwatch.ElapsedTicks - frameStartTime;
                frameStartTime += elapsedTime;

                float elapsedSeconds = (float)elapsedTime / (float)Stopwatch.Frequency;

                input.Update();
                ConsoleRasterizer.UpdateConsoleTitle(elapsedSeconds);

                inputDebugWindow.Update(elapsedSeconds);
                fpsCounter.Update(elapsedSeconds);
                fpsCounter.GenerateSprites();

                SMALL_RECT newWindowSize;
                if (ConsoleRasterizer.HandleWindowResize(out newWindowSize))
                {
                    frameBuffer.RecreateBuffers(
                        frameBuffer.Buffers.Count,
                        ConsoleRasterizer.WindowWidth,
                        ConsoleRasterizer.WindowHeight
                    );
                }

                try
                {
                    frameBuffer.ClearBuffer(' ', (ushort)(CHAR_INFO_ATTRIBUTE.BG_RED | CHAR_INFO_ATTRIBUTE.FG_RED));
                }
                catch (System.IndexOutOfRangeException)
                {
                    ConsoleRasterizer.InitalizeSurface(ConsoleRasterizer.WindowWidth, ConsoleRasterizer.WindowHeight, fontWidth, fontHeight);
                    frameBuffer.RecreateBuffers(frameBuffer.Buffers.Count, ConsoleRasterizer.WindowWidth, ConsoleRasterizer.WindowHeight);
                }

                frameBuffer += inputDebugWindow.Render();
                frameBuffer += fpsCounter.Sprites[0];

                try
                {
                    ConsoleRasterizer.CopyBufferToScreen(frameBuffer.CurrentBuffer, frameBuffer.Width, frameBuffer.Height);
                }
                catch (System.IndexOutOfRangeException)
                {
                    ConsoleRasterizer.InitalizeSurface(ConsoleRasterizer.WindowWidth, ConsoleRasterizer.WindowHeight, fontWidth, fontHeight);
                    frameBuffer.RecreateBuffers(frameBuffer.Buffers.Count, ConsoleRasterizer.WindowWidth, ConsoleRasterizer.WindowHeight);
                }

            } while (true);

        }
    }
}

