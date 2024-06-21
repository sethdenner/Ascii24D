using Engine.Core.ECS;
using Engine.Native;
using System.Numerics;

namespace Engine.Render {
    public class ConsoleRenderSystem : System<ConsoleRenderComponent> {
        public override void Cleanup() {}

        public override void SetupComponent(
            ref ConsoleRenderComponent component
        ) {
        }

        public override void BeforeUpdates(long step, bool headless = false) {
        }

        public override void UpdateComponent(
            ref ConsoleRenderComponent component,
            long step,
            bool headless = false
        ) {
            if (headless) {
                return;
            }
            if (
                Native.Native.HandleWindowResize(out SMALL_RECT newWindowSize)
            ) {
                component.Framebuffer = ResizeWindow(
                    newWindowSize,
                    component.FontWidth,
                    component.FontHeight
                );
            }
            /*
            try {
                Array.Fill(
                    component.Framebuffer,
                    component.FillPixel
                );
            } catch (System.IndexOutOfRangeException) {
                Native.Native.HandleWindowResize(out newWindowSize);
                component.Framebuffer = ResizeWindow(
                    newWindowSize,
                    component.FontWidth,
                    component.FontHeight
               );
            }
            */
            try {
                Native.Native.CopyBufferToScreen(
                    component.Framebuffer,
                    component.FramebufferWidth,
                    component.FramebufferHeight,
                    0,
                    0
                );
            } catch (System.IndexOutOfRangeException) {
                Native.Native.HandleWindowResize(out newWindowSize);
                component.Framebuffer = ResizeWindow(
                    newWindowSize,
                    component.FontWidth,
                    component.FontHeight
                );
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static ConsolePixel[] CreateFramebuffer(
            int width,
            int height
        ) {
            return new ConsolePixel[width * height];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newWindowSize"></param>
        /// <returns></returns>
        public ConsolePixel[] ResizeWindow(
            SMALL_RECT newWindowSize,
            int fontWidth,
            int fontHeight
        ) {
            if (newWindowSize.Right < 1) {
                newWindowSize.Right = 1;
            }
            if (newWindowSize.Bottom < 1) {
                newWindowSize.Bottom = 1;
            }

            Native.Native.InitalizeSurface(
               newWindowSize.Right - newWindowSize.Left,
               newWindowSize.Bottom - newWindowSize.Top,
               fontWidth,
               fontHeight
            );

            MessageOutbox.Add(new ApplicationWindowResizedMessage(
                newWindowSize
            ));
            return CreateFramebuffer(
               newWindowSize.Right,
               newWindowSize.Bottom
            );
        }
    }
}
