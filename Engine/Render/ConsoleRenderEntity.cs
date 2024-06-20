using Engine.Core;
using Engine.Core.ECS;
using Engine.Native;
using System.Numerics;
using static Engine.Render.SpriteSystem;
namespace Engine.Render {
    public class ConsoleRenderEntity : Entity {
        public ConsoleRenderEntity(
            int width,
            int height,
            int fontWidth,
            int fontHeight,
            ConsolePixel fillPixel,
            PaletteInfo[] palette
        ) {
            Native.Native.CreateConsoleWindow();
            Native.Native.InitalizeSurface(
                width,
                height,
                fontWidth,
                fontHeight
            );
            Console.Clear();
            Native.Native.SetScreenColors(palette);

            AddComponent(new ConsoleRenderComponent(
                width,
                height,
                fontWidth,
                fontHeight,
                fillPixel,
                new ConsolePixel[width * height],
                palette
            ));

            Messenger<ApplicationWindowResizedMessage.Delegate>.Register(
                ResizeFramebuffer
            );
            Messenger<UpdateFramebufferMessage.Delegate>.Register(
                UpdateFramebuffer
            );
        }

        public void ResizeFramebuffer(SMALL_RECT newSizeRect) {
            var component = GetComponent<ConsoleRenderComponent>();
            int width = newSizeRect.Right - newSizeRect.Left;
            int height = newSizeRect.Bottom - newSizeRect.Top;
            component.Framebuffer = new ConsolePixel[width * height];
            component.FramebufferWidth = width;
            component.FramebufferHeight = height;
            SetComponent(component);
        }

        public void UpdateFramebuffer(int entityID, ConsolePixel[] framebuffer) {
            if (EntityID == entityID) {
                var component = GetComponent<ConsoleRenderComponent>();
                component.Framebuffer = framebuffer;
                SetComponent(component);
            }
        }
    }
}
