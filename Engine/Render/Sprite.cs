using Engine.Native;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Render
{
    /// <summary>
    /// A <c>Sprite</c> is a collection of buffer pixels  meant to be rendered
    /// to a console buffer as a square block of text and attributes.
    /// </summary>    
    public class Sprite {
        /// <summary>
        /// <c>Sprite</c> default constructor.
        /// </summary>
        public Sprite() {
            Width = 0;
            Height = 0;
            OffsetX = 0;
            OffsetY = 0;
            BufferPixels = [];
        }
        /// <summary>
        /// <c>Sprite</c> constructor taking params that define the render
        /// properties.
        /// </summary>
        /// <param name="width">
        /// The width of the sprite in console pixels.
        /// </param>
        /// <param name="height">
        /// The height of the sprite in console pixels.
        /// </param>
        /// <param name="offsetX">
        /// The number of pixels offset on the x-axis the sprite should be
        /// rendered onto another sprite.
        /// </param>
        /// <param name="offsetY">
        /// The number of pixels offset on the y-axis the sprite should be
        /// rendered onto another sprite.
        /// </param>
        /// <param name="edgeBehavior">
        /// How to handle rendering the sprite when rendering would happen
        /// outside the bounds of the render target. Pixel can be discarded or
        /// wrapped to the other side of the render target.
        /// </param>
        public Sprite(
            int width,
            int height,
            int offsetX = 0,
            int offsetY = 0
        ) {
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
            BufferPixels = new ConsolePixel[width * height];
        }
        /// <summary>
        /// <c>MergeSprite</c> blends the provided sprite onto the pixel buffer.
        /// </summary>
        /// <param name="sprite">
        /// An instance of <c>Sprite</c> to blend.
        /// </param>
        /// <returns><c>this</c></returns>
        public Sprite MergeSprite(Sprite sprite) {
            int destTop = Math.Clamp(sprite.OffsetY, 0, Height);
            int destLeft = Math.Clamp(sprite.OffsetX, 0, Width);
            int destBottom = Math.Clamp(
                sprite.OffsetY + sprite.Height,
                0,
                Height
            );
           
            int destRight = Math.Clamp(
                sprite.OffsetX + sprite.Width,
                0,
                Width
            );
            int srcTop = Math.Clamp(
                Height - (sprite.OffsetY + Height),
                0,
                sprite.Height
            );
            int srcLeft = Math.Clamp(
                Width -  (sprite.OffsetX + Width),
                0,
                sprite.Width
            );
            int srcBottom = Math.Clamp(
                Math.Abs(sprite.OffsetY - Height),
                0,
                sprite.Height
            );
            int srcRight = Math.Clamp(
                Math.Abs(sprite.OffsetX - Width),
                0,
                sprite.Width
            );
            int height = Math.Min(
                Math.Abs(destBottom - destTop),
                Math.Abs(srcBottom - srcTop)
            );
            int width = Math.Min(
                Math.Abs(destRight - destLeft),
                Math.Abs(srcRight - srcLeft)
            );
            for (int y = 0; y < height; ++y) {
                int destStart = destLeft + ((y + destTop) * Width);
                int srcStart = srcLeft + ((y + srcTop) * sprite.Width);
                int destEnd = destStart + width;
                int srcEnd = srcStart + width;
                sprite.BufferPixels.AsSpan()[srcStart..srcEnd].CopyTo(
                    BufferPixels.AsSpan()[destStart..destEnd]
                );
            }
            return this;
        }
        /// <summary>
        /// An array of <c>Pixel</c> instances representing the sprite.
        /// </summary>
        public ConsolePixel[] BufferPixels {
            get; set;
        }
        /// <summary>
        /// The total width of the sprite.
        /// </summary>
        public int Width {
            get; set;
        }
        /// <summary>
        /// The total height of the sprite.
        /// </summary>
        public int Height {
            get; set;
        }
        /// <summary>
        /// The offset in the x-axis the sprite should be rendered.
        /// </summary>
        public int OffsetX {
            get; set;
        }
        /// <summary>
        /// The offset in the y-axis the sprite should be rendered.
        /// </summary>
        public int OffsetY {
            get; set;
        }
        /// <summary>
        /// <c>SetPixel</c> method sets the pixel in the pixel buffer at
        /// position (x, y) to the provided <c>Pixel</c> instance.
        /// </summary>
        /// <param name="x">
        /// The position on the <c>Sprite</c> in the x axis.
        /// </param>
        /// <param name="y">
        /// The position on the <c>Sprite</c> in the y axis.
        /// </param>
        /// <param name="bufferPixel">
        /// An instance of <c>Pixel</c> that will be copied to the pixel buffer
        /// at the specified location.
        /// </param>
        /// <returns>
        /// <c>true</c> if the pixel was set. <c>false</c> if the pixel was
        /// rejected for some reason.
        /// </returns>
        public bool SetPixel(
            int x,
            int y,
            ConsolePixel bufferPixel
        ) {
            if (Width <= x || 0 > x || Height <= y || 0 > y)
                return false;

            BufferPixels[x + y * Width] = bufferPixel;

            return true;
        }
        /// <summary>
        /// <c>SetPixel</c> method sets the pixel in the pixel buffer at
        /// position (x, y) to the provided <c>Pixel</c> instance.
        /// </summary>
        /// <param name="i">
        /// The index into the pixel buffer to set.
        /// </param>
        /// <param name="bufferPixel">
        /// An instance of <c>Pixel</c> that will be copied to the pixel buffer
        /// at the specified location.
        /// </param>
        /// <returns>
        /// <c>true</c> if the pixel was set. <c>false</c> if the pixel was
        /// rejected for some reason.
        /// </returns>
        public bool SetPixel(
            int i,
            ConsolePixel bufferPixel
        ) {
            if (i < 0 || i >= BufferPixels.Length)
                return false;

            BufferPixels[i] = bufferPixel;

            return true;
        }
        /// <summary>
        /// <c>Fill</c> copies a provided instance of <c>Pixel</c> to the entire
        /// pixel buffer.
        /// </summary>
        /// <param name="bufferPixel">
        /// The <c>Pixel</c> instance tro copy into the pixel buffer.
        /// </param>
        public void Fill(ConsolePixel bufferPixel) {
            BufferPixels.AsSpan().Fill(bufferPixel);
        }
    }
}
