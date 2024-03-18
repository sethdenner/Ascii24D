using System.Diagnostics;
using SharpDX.DirectInput;
using Engine.Input;
using Engine.Characters;
using Engine.Characters.UI;
using Engine.Render;
using Engine.Native;

namespace DirectInputDebugDemo
{
    /// <summary>
    /// Program entry point class.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Application entry point function.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>
        /// Application return code. 0 means OK. Anything else indicates an
        /// error occured.
        /// </returns>
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

            // These values become unreliable once we start
            // changing the console window with the native 
            // windows api. Initialize with them here but we
            // need to keep track of these values ourselves.
            Native.CreateConsoleWindow();
            Native.InitalizeSurface(
                80,
                50,
                fontWidth,
                fontHeight
            );

            Console.Clear();

            Sprite<CHAR_INFO> framebuffer = new(
                Native.WindowWidth,
                Native.WindowHeight
            );

            CharacterFrameCounter fpsCounter = new CharacterFrameCounter();
            UIWindowTextDebugInput inputDebugWindow = (
                new UIWindowTextDebugInput(input)
            );

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
                    framebuffer = new Sprite<CHAR_INFO>(
                        newWindowSize.Right - newWindowSize.Left,
                        newWindowSize.Bottom - newWindowSize.Top,
                        newWindowSize.Left,
                        newWindowSize.Top
                    );
                }

                try
                {
                    
                    framebuffer.Fill(new CHAR_INFO() {
                        Char = ' ',
                        Attributes = (ushort)(
                            CHAR_INFO_ATTRIBUTE.BG_RED |
                            CHAR_INFO_ATTRIBUTE.FG_RED
                        )
                    });
                }
                catch (System.IndexOutOfRangeException)
                {
                    Native.InitalizeSurface(
                        Native.WindowWidth,
                        Native.WindowHeight,
                        fontWidth,
                        fontHeight
                    );
                    framebuffer = new Sprite<CHAR_INFO>(
                       newWindowSize.Right - newWindowSize.Left,
                       newWindowSize.Bottom - newWindowSize.Top,
                       newWindowSize.Left,
                       newWindowSize.Top
                   );
                }

                framebuffer += inputDebugWindow.Render();
                framebuffer.EdgeBehavior = EdgeBehavior.WRAP;
                framebuffer += fpsCounter.Sprites[0];
                framebuffer.EdgeBehavior = EdgeBehavior.CLAMP;

                try
                {
                    Native.CopyBufferToScreen(
                        framebuffer.BufferPixels,
                        framebuffer.Width,
                        framebuffer.Height,
                        framebuffer.OffsetX,
                        framebuffer.OffsetY
                    );
                }
                catch (System.IndexOutOfRangeException)
                {
                    Native.InitalizeSurface(
                        Native.WindowWidth,
                        Native.WindowHeight,
                        fontWidth,
                        fontHeight
                    );
                    framebuffer = new Sprite<CHAR_INFO>(
                        newWindowSize.Right - newWindowSize.Left,
                        newWindowSize.Bottom - newWindowSize.Top,
                        newWindowSize.Left,
                        newWindowSize.Top
                    );
                }
            } while (true);
        }
    }
}
