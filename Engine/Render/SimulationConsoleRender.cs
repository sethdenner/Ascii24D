using Engine.Core;
using Engine.Native;

namespace Engine.Render {
    public class SimulationConsoleRender(
        int width,
        int height,
        int fontWidth,
        int fontHeight,
        ConsolePixel fillPixel
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
        public ConsolePixel FillPixel {
            get; set;
        } = fillPixel;
        public Sprite Framebuffer {
            get; set;
        } = new(width, height);

        public void Cleanup(IApplicationState state) { }

        public void Setup(IApplicationState state) {
            Native.Native.CreateConsoleWindow();
            Native.Native.InitalizeSurface(
                Width,
                Height,
                FontWidth,
                FontHeight
            );
            Console.Clear();
            Native.Native.SetScreenColors(state.CurrentScene.Palette);
            Framebuffer = CreateFramebuffer(
               Framebuffer.Width,
               Framebuffer.Height,
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
                Framebuffer = ResizeWindow(newWindowSize);
                state.FramebufferHeight = Framebuffer.Height;
                state.FramebufferWidth = Framebuffer.Width;
            }
            try {
                Framebuffer.Fill(FillPixel);
            } catch (System.IndexOutOfRangeException) {
                Native.Native.HandleWindowResize(out newWindowSize);
                Framebuffer = ResizeWindow(newWindowSize);
                state.FramebufferHeight = Framebuffer.Height;
                state.FramebufferWidth = Framebuffer.Width;
            }
            state.CurrentScene.Render(Framebuffer, state.ViewMatrix);
            try {
                Native.Native.CopyBufferToScreen(
                    Framebuffer.BufferPixels,
                    Framebuffer.Width,
                    Framebuffer.Height,
                    0,
                    0
                );
            } catch (System.IndexOutOfRangeException) {
                Native.Native.HandleWindowResize(out newWindowSize);
                Framebuffer = ResizeWindow(newWindowSize);
                state.FramebufferHeight = Framebuffer.Height;
                state.FramebufferWidth = Framebuffer.Width;
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
        public Sprite ResizeWindow(SMALL_RECT newWindowSize) {
            if (newWindowSize.Right < 1) { newWindowSize.Right = 1; }
            if (newWindowSize.Bottom < 1) { newWindowSize.Bottom = 1; }

            Native.Native.InitalizeSurface(
               newWindowSize.Right - newWindowSize.Left,
               newWindowSize.Bottom - newWindowSize.Top,
               FontWidth,
               FontHeight
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
