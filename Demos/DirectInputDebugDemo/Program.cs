using System.Diagnostics;
using SharpDX.DirectInput;
using Engine.Input;
using Engine.Characters;
using Engine.Characters.UI;
using Engine.Render;
using Engine.Native;
using Engine.Core;

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
        static int Main(string[] args)
        {
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

            Sprite framebuffer = CreateFramebuffer(
                Native.WindowWidth,
                Native.WindowHeight,
                0,
                0
            );

            CharacterFrameCounter fpsCounter = new CharacterFrameCounter();
            UIWindowTextDebugInput inputDebugWindow = (
                new UIWindowTextDebugInput(input)
            );
            Map demoMap = new Map();
            demoMap.AddCharacter(inputDebugWindow);
            demoMap.AddCharacter(fpsCounter);

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

                input.Update();
                Native.UpdateConsoleTitle(elapsedSeconds);

                inputDebugWindow.Update(elapsedSeconds);
                fpsCounter.Update(elapsedSeconds);
                fpsCounter.GenerateSprites();

                SMALL_RECT newWindowSize;
                if (Native.HandleWindowResize(out newWindowSize))
                {
                    framebuffer = ResizeWindow(newWindowSize);
                }

                try
                {
                    framebuffer.Fill(new Pixel(
                        new Engine.Native.ConsoleColor() {
                            R = (byte)255,
                            G = (byte)0,
                            B = (byte)0
                        },
                        new Engine.Native.ConsoleColor() {
                            R = (byte)255,
                            G = (byte)0,
                            B = (byte)0
                        },
                        (byte)' '
                    ));
                }
                catch (System.IndexOutOfRangeException)
                {
                    framebuffer = ResizeWindow(newWindowSize);
                }

                framebuffer.MergeSprite(inputDebugWindow.Render());
                framebuffer.EdgeBehavior = EdgeBehavior.WRAP;
                framebuffer.MergeSprite(fpsCounter.Sprites[0]);
                framebuffer.EdgeBehavior = EdgeBehavior.CLAMP;

                try
                {
                    Native.CopyBufferToScreenVT(
                        framebuffer.GetNativePixelBuffer(),
                        framebuffer.Width,
                        framebuffer.Height,
                        framebuffer.OffsetX,
                        framebuffer.OffsetY
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
