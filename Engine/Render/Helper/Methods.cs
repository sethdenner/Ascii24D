using Engine.Native;
using System.Numerics;

namespace Engine.Render.Helper {
    public static class Methods {
        public static void BlendTextures(
            ConsolePixel[] destination,
            int destinationWidth,
            int destinationHeight,
            ConsolePixel[]  source,
            int sourceWidth,
            int sourceHeight,
            Vector2 texturePosition
        ) {
            int u = (int)Math.Floor(texturePosition.X);
            int v = (int)Math.Floor(texturePosition.Y);
            int destinationTop = Math.Clamp(
                v,
                0,
                destinationHeight
            );
            int destinationLeft = Math.Clamp(
                u,
                0,
                destinationWidth
            );
            int destinationBottom = Math.Clamp(
                v + sourceHeight,
                0,
                destinationHeight
            );

            int destinationRight = Math.Clamp(
                u + sourceWidth,
                0,
                destinationWidth
            );
            int sourceTop = Math.Clamp(
                destinationHeight - (v + destinationHeight),
                0,
                sourceHeight
            );
            int sourceLeft = Math.Clamp(
                destinationWidth - (u + destinationWidth),
                0,
                sourceWidth
            );
            int sourceBottom = Math.Clamp(
                Math.Abs(v - destinationHeight),
                0,
                sourceHeight
            );
            int sourceRight = Math.Clamp(
                Math.Abs(u - destinationWidth),
                0,
                sourceWidth
            );
            int height = Math.Min(
                Math.Abs(destinationBottom - destinationTop),
                Math.Abs(sourceBottom - sourceTop)
            );
            int width = Math.Min(
                Math.Abs(destinationRight - destinationLeft),
                Math.Abs(sourceRight - sourceLeft)
            );
            for (int y = 0; y < height; ++y) {
                int destinationStart = destinationLeft + (
                    (y + destinationTop) * destinationWidth
                );
                int sourceStart = sourceLeft + ((y + sourceTop) * sourceWidth);
                int destinationEnd = destinationStart + width;
                int sourceEnd = sourceStart + width;
                source.AsSpan()[sourceStart..sourceEnd].CopyTo(
                    destination.AsSpan()[
                        destinationStart..destinationEnd
                    ]
                );
            }
        }
    }
}
