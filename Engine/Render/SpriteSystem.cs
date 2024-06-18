using Engine.Core;
using Engine.Core.ECS;
using Engine.Native;
using System.Numerics;
using System.Xml.Xsl;

namespace Engine.Render {
    public class SpriteSystem(
        int framebufferWidth,
        int framebufferHeight
    ) : System<SpriteComponent> {
        public delegate void UpdateFramebufferMessage(
            ConsolePixel[] framebuffer
        );

        public ConsolePixel[] Framebuffer = new Native.ConsolePixel[
            framebufferWidth * framebufferHeight
        ];
        public int FramebufferWidth = framebufferWidth;
        public int FramebufferHeight = framebufferHeight;
        public Matrix4x4 WorldCameraViewport = Matrix4x4.Identity;

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
            Sprite sprite = new(
                component.Width,
                component.Height,
                (int)Math.Ceiling(screenPosition.X),
                (int)Math.Ceiling(screenPosition.Y)
            );
            component.BufferPixels.AsSpan().CopyTo(
                sprite.BufferPixels.AsSpan()
            );
            Helper.Methods.BlendTextures(
                Framebuffer,
                FramebufferWidth,
                FramebufferHeight,
                component.BufferPixels,
                component.Width,
                component.Height,
                screenPosition
            );
        }

        public override void BeforeUpdates(long step,  bool headless = false) {
            if (headless) { return; }

            Framebuffer = new ConsolePixel[Framebuffer.Length];
        }

        public override void AfterUpdates(long step, bool headless = false) {
            if (headless) { return; }

            Messenger<UpdateFramebufferMessage>.Trigger?.Invoke(Framebuffer);
        }
    }
}
