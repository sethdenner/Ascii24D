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

            Messenger<UpdateFramebufferMessage>.Register(UpdateFramebuffer);
        }

        public void UpdateFramebuffer(ConsolePixel[] framebuffer) {
            var component = GetComponent<ConsoleRenderComponent>();
            component.Framebuffer = framebuffer;
        }
    }
}
