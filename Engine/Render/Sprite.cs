using Engine.Native;

namespace Engine.Render
{
    /// <summary>
    /// Enumeration used for specifying how to handle border overflow during
    /// <c>Sprite</c> addition.
    /// </summary>  
    public enum EdgeBehavior
    {
        CLAMP,
        WRAP
    }
    /// <summary>
    /// A <c>Sprite</c> is a collection of buffer pixels  meant to be rendered
    /// to a console buffer as a square block of text and attributes.
    /// </summary>    
    public class Sprite
    {
        /// <summary>
        /// <c>Sprite</c> default constructor.
        /// </summary>
        public Sprite()
        {
            Width = 0; Height = 0;
            OffsetX = 0; OffsetY = 0;
            EdgeBehavior = EdgeBehavior.CLAMP;
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
            int offsetY = 0,
            EdgeBehavior edgeBehavior = EdgeBehavior.CLAMP
        )
        {
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
            BufferPixels = new Pixel[width * height];
            EdgeBehavior = edgeBehavior;
        }
        /// <summary>
        /// <c>DepthTest</c> method checks the <c>PixelDepth</c> of the
        /// <c>Pixel</c> in the <c>Sprite</c> a position (x, y) against the
        /// provided depth value.
        /// </summary>
        /// <param name="x">
        /// The position on the <c>Sprite</c> in the x axis.
        /// </param>
        /// <param name="y">
        /// The position on the <c>Sprite</c> in the y axis.
        /// </param>
        /// <param name="depth"></param>
        /// The depth value to test against the current pixel at position
        /// (x, y).
        /// <returns>
        /// Returns <c>true</c> if <paramref name="depth"/> is greater than the
        /// current <c>PixelDepth</c> at position (x, y). Otherwise <c>false</c>
        /// is returned.
        /// </returns>
        public bool DepthTest(
            int x,
            int y,
            int depth
        )
        {
            return DepthTest(x + (y * Width), depth);
        }
        /// <summary>
        /// <c>DepthTest</c> method checks the <c>PixelDepth</c> of the
        /// <c>Pixel</c> in the <c>Sprite</c> a position (x, y) against the
        /// provided depth value.
        /// </summary>
        /// <param name="i">
        /// The index into the pixel buffer to test.
        /// </param>
        /// <param name="depth"></param>
        /// The depth value to test against the current pixel at position
        /// (x, y).
        /// <returns>
        /// Returns <c>true</c> if <paramref name="depth"/> is greater than the
        /// current <c>PixelDepth</c> at position (x, y). Otherwise <c>false</c>
        /// is returned.
        /// </returns>
        public bool DepthTest(
            int i,
            int depth
        )
        {
            if (depth >= BufferPixels[i].PixelDepth)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// <c>MergeSprite</c> blends the provided sprite onto the pixel buffer.
        /// </summary>
        /// <param name="sprite">
        /// An instance of <c>Sprite</c> to blend.
        /// </param>
        /// <returns><c>this</c></returns>
        public Sprite MergeSprite(
            Sprite sprite
        )
        {
            for (int y = 0; y < sprite.Height; ++y)
            {
                for (int x = 0; x < sprite.Width; ++x)
                {
                    int xCoord = x + sprite.OffsetX;
                    int yCoord = y + sprite.OffsetY;
                    if (
                        EdgeBehavior.CLAMP ==
                        sprite.EdgeBehavior
                    )
                    {
                        // Handle clamping.
                        if (xCoord >= Width || xCoord < 0) continue;
                        if (yCoord >= Height || yCoord < 0) continue;
                    }
                    else if (
                        EdgeBehavior.WRAP ==
                        sprite.EdgeBehavior
                    )
                    {
                        // Handle wrapping.
                        if (xCoord >= Width || xCoord < 0) xCoord = (
                            (Width - Math.Abs(xCoord)) % Width
                        );
                        if (yCoord >= Height || yCoord < 0) yCoord = (
                            (Height - Math.Abs(yCoord)) % Height
                        );
                    }
                    int rhsBufferIndex = x + y * sprite.Width;
                    int lhsBufferIndex = xCoord + yCoord * Width;
                    Pixel rhsPixel = sprite.BufferPixels[rhsBufferIndex];
                    DepthTest(xCoord, yCoord, rhsPixel.PixelDepth);
                    BufferPixels[lhsBufferIndex] = rhsPixel;
                }
            }
            OffsetX = sprite.OffsetX;
            OffsetY = sprite.OffsetY;
            return this;
        }
        /// <summary>
        /// An array of <c>Pixel</c> instances representing the sprite.
        /// </summary>
        public Pixel[] BufferPixels { get; }
        /// <summary>
        /// The total width of the sprite.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// The total height of the sprite.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// The offset in the x-axis the sprite should be rendered.
        /// </summary>
        public int OffsetX { get; set; }
        /// <summary>
        /// The offset in the y-axis the sprite should be renderd.
        /// </summary>
        public int OffsetY { get; set; }
        /// <summary>
        /// <c>EdgeBehavior</c> specifies how to handle rendering outside of the
        /// bounds of a render target.
        /// </summary>
        public EdgeBehavior EdgeBehavior { get; set; }
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
            Pixel bufferPixel
        )
        {
            if (Width <= x || 0 > x || Height <= y || 0 > y)
                return false;

            if (!DepthTest(x, y, bufferPixel.PixelDepth))
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
            Pixel bufferPixel
        )
        {
            if (i < 0 || i >= BufferPixels.Length)
                return false;

            // Pass in i as x and 0 as y. Math works out.
            if (!DepthTest(i, bufferPixel.PixelDepth))
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
        public void Fill(Pixel bufferPixel)
        {
            for (var i = 0; i < BufferPixels.Length; ++i)
            {
                BufferPixels[i] = bufferPixel;
            }
        }
        /// <summary>
        /// <c>GetNativePixelBuffer</c> converts the pixel buffer from
        /// <c>Pixel</c> instances to the marshallable <c>ConsolePixel</c>
        /// struct.
        /// </summary>
        /// <returns>
        /// An array of <c>ConsolePixel</c> values.
        /// </returns>
        public ConsolePixel[] GetNativePixelBuffer()
        {
            ConsolePixel[] buffer = new ConsolePixel[BufferPixels.Length];
            for (int i = 0; i < BufferPixels.Length; ++i)
            {
                buffer[i] = BufferPixels[i].PixelDescription;
            }
            return buffer;
        }
    }
}
