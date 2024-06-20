using Engine.Core;
using Engine.Core.ECS;
using Engine.Native;
using System.Numerics;

namespace Engine.Render {
    public class SpriteSystem : System<SpriteComponent> {
        public ConsolePixel[] Framebuffer;
        public int ConsoleRenderEntityID;
        public int FramebufferWidth;
        public int FramebufferHeight;
        public Matrix4x4 WorldCameraViewport;

        public SpriteSystem(
            int consoleRenderEntityID,
            int framebufferWidth,
            int framebufferHeight
        ) {
            Framebuffer = new Native.ConsolePixel[
                framebufferWidth * framebufferHeight
            ];
            ConsoleRenderEntityID = consoleRenderEntityID;
            FramebufferWidth = framebufferWidth;
            FramebufferHeight = framebufferHeight;
            WorldCameraViewport = Matrix4x4.Identity;

            Message.Register<ApplicationWindowResizedMessage.Delegate>(
                ResizeFramebuffer
            );
        }
        public override void Cleanup() {
            throw new NotImplementedException();
        }

        public override void SetupComponent(ref SpriteComponent component) { }

        public override void UpdateComponent(
            ref SpriteComponent component,
            long step,
            bool headless = false
        ) {
            if (headless) { return; }

            Vector3 screenPosition;
            if (component.TransformToScreenSpace) {
                screenPosition = Vector3.Transform(
                    component.ModelPosition,
                    WorldCameraViewport
                );
            } else {
                screenPosition = component.ModelPosition;
            }
            Vector2 texturePosition = new(
                screenPosition.X,
                screenPosition.Y
            );
            Helper.Methods.BlendTextures(
                Framebuffer,
                FramebufferWidth,
                FramebufferHeight,
                component.BufferPixels,
                component.Width,
                component.Height,
                texturePosition
            );
        }

        public override void BeforeUpdates(long step,  bool headless = false) {
            if (headless) { return; }

            Framebuffer = new ConsolePixel[Framebuffer.Length];
        }

        public override void AfterUpdates(long step, bool headless = false) {
            if (headless) { return; }

            new UpdateFramebufferMessage(
                ConsoleRenderEntityID,
                Framebuffer
            ).Send();
        }

        public void ResizeFramebuffer(SMALL_RECT newSizeRect) {
            int width = newSizeRect.Right - newSizeRect.Left;
            int height = newSizeRect.Bottom - newSizeRect.Top;
            Framebuffer = new ConsolePixel[width * height];
            FramebufferWidth = width;
            FramebufferHeight = height;
        }

    }
}
