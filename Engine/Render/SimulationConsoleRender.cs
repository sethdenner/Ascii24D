using Engine.Core;
using Engine.Native;

namespace Engine.Render {
    public class SimulationConsoleRender(
        int width,
        int height,
        int fontWidth,
        int fontHeight
    ) : ISimulation {
        public int Width {
            get; set;
        } = width;
        public int Height {
            get; set;
        } = height;
        public int FontWidth {
            get; set;
        } = fontWidth;
        public int FontHeight {
            get; set;
        } = fontHeight;

        public void Cleanup(IApplicationState state) {
        }

        public void Setup(IApplicationState state) {
            Native.Native.CreateConsoleWindow();
            Native.Native.InitalizeSurface(
                Width,
                Height,
                FontWidth,
                FontHeight
            );
            Console.Clear();
            Native.Native.SetScreenColors(
                5,
                state.CurrentScene.Palette
            );
            state.Framebuffer = CreateFramebuffer(
               Native.Native.WindowWidth,
               Native.Native.WindowHeight,
               0,
               0
           );
        }

        public void Simulate(
            IApplicationState state,
            long step,
            bool headless = false
        ) {
            if (headless) {
                return;
            }
            if (
                Native.Native.HandleWindowResize(out SMALL_RECT newWindowSize)
            ) {
                state.Framebuffer = ResizeWindow(newWindowSize);
            }
            try {
                state.Framebuffer.Fill(new ConsolePixel() {
                    ForegroundColorIndex = 2,
                    BackgroundColorIndex = 2,
                    CharacterCode = (byte)' '
                });
            } catch (System.IndexOutOfRangeException) {
                Native.Native.HandleWindowResize(out newWindowSize);
                state.Framebuffer = ResizeWindow(newWindowSize);
            }
            state.CurrentScene.Render(state.Framebuffer);
            try {
                Native.Native.CopyBufferToScreen(
                    state.Framebuffer.BufferPixels,
                    state.Framebuffer.Width,
                    state.Framebuffer.Height,
                    0,
                    0
                );
            } catch (System.IndexOutOfRangeException) {
                Native.Native.HandleWindowResize(out newWindowSize);
                state.Framebuffer = ResizeWindow(newWindowSize);
            }
        }

        public Task SimulateAsync(
            IApplicationState state,
            long step,
            bool headless = false
        ) {
            throw new NotImplementedException();
        }
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
        ) {
            return new Sprite(
                width,
                height,
                left,
                top
            );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newWindowSize"></param>
        /// <returns></returns>
        public static Sprite ResizeWindow(SMALL_RECT newWindowSize) {
            Native.Native.InitalizeSurface(
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
    }
}
