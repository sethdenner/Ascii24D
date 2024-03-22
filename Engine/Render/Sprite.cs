using Engine.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="edgeBehavior"></param>
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
            if (
                depth >=
                BufferPixels[x + (y * Width)].PixelDepth
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Sprite operator +(Sprite lhs, Sprite rhs)
        {
            Sprite mergedSprite = new Sprite(
                int.Max(lhs.Width, rhs.Width + rhs.OffsetX),
                int.Max(lhs.Height, rhs.Height + rhs.OffsetY),
                lhs.OffsetX, lhs.OffsetY
            );
            mergedSprite.MergeSprite(lhs);
            mergedSprite.MergeSprite(rhs);
            return mergedSprite;
        }
        /// <summary>
        /// 
        /// </summary>
        public Pixel[] BufferPixels { get; }
        /// <summary>
        /// 
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int OffsetX { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int OffsetY { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EdgeBehavior EdgeBehavior { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="bufferPixel"></param>
        /// <returns></returns>
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
        public bool SetPixel(
            int i,
            Pixel bufferPixel
        )
        {
            if (i < 0 || i >= BufferPixels.Length)
                return false;

            // Pass in i as x and 0 as y. Math works out.
            if (!DepthTest(i, 0, bufferPixel.PixelDepth))
                return false;

            BufferPixels[i] = bufferPixel;

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="charInfo"></param>
        public void Fill(Pixel bufferPixel)
        {
            for (var i = 0; i < BufferPixels.Length; ++i)
            {
                BufferPixels[i] = bufferPixel;
            }
        }
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
