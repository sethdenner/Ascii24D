using Engine.Native;
using System.Numerics;

namespace Engine.Render {
    public struct ConsoleRenderComponent(
        int framebufferWidth,
        int framebufferHeight,
        int fontWidth,
        int fontHeight,
        ConsolePixel fillPixel,
        ConsolePixel[] framebuffer,
        PaletteInfo[] palette
    ) {
        public int FramebufferWidth = framebufferWidth;
        public int FramebufferHeight = framebufferHeight;
        public int FontWidth = fontWidth;
        public int FontHeight = fontHeight;
        public ConsolePixel[] Framebuffer = framebuffer;
        public ConsolePixel FillPixel = fillPixel;
        public PaletteInfo[] Palette = palette;
        public Matrix4x4 ViewportMatrix = Matrix4x4.Identity;
    }
}
