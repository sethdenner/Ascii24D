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
            Vector3 texturePosition
        ) {
            int screenX = (int)Math.Floor(texturePosition.X);
            int screenY = (int)Math.Floor(texturePosition.Y);
            int destinationTop = Math.Clamp(
                screenY,
                0,
                destinationHeight
            );
            int destinationLeft = Math.Clamp(
                screenX,
                0,
                destinationWidth
            );
            int destinationBottom = Math.Clamp(
                screenY + sourceHeight,
                0,
                destinationHeight
            );

            int destinationRight = Math.Clamp(
                screenX + sourceWidth,
                0,
                destinationWidth
            );
            int sourceTop = Math.Clamp(
                destinationHeight - (screenY + destinationHeight),
                0,
                sourceHeight
            );
            int sourceLeft = Math.Clamp(
                destinationWidth - (screenX + destinationWidth),
                0,
                sourceWidth
            );
            int sourceBottom = Math.Clamp(
                Math.Abs(screenY - destinationHeight),
                0,
                sourceHeight
            );
            int sourceRight = Math.Clamp(
                Math.Abs(screenX - destinationWidth),
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
