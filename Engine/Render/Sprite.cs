using Engine.Native;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <typeparam name="BufferPixelType"></typeparam>
    /// </summary>    
    public class Sprite<BufferPixelType>
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
            BufferPixels = new BufferPixelType[width * height];
            EdgeBehavior = edgeBehavior;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public Sprite<BufferPixelType> MergeSprite(
            Sprite<BufferPixelType> sprite
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
                        EdgeBehavior
                    )
                    {
                        // Handle clamping.
                        if (xCoord >= Width || xCoord < 0) continue;
                        if (yCoord >= Height || yCoord < 0) continue;
                    }
                    else if (
                        EdgeBehavior.WRAP ==
                        EdgeBehavior
                    )
                    {
                        // Handle wrapping.
                        if (xCoord >= Width || xCoord < 0) xCoord = (
                            Width - Math.Abs(xCoord) % Width
                        );
                        if (yCoord >= Height || yCoord < 0) yCoord = (
                            Height - Math.Abs(yCoord) % Height
                        );
                    }
                    int lhsBufferIndex = xCoord + yCoord * Width;
                    int rhsBufferIndex = x + y * sprite.Width;
                    BufferPixels[lhsBufferIndex] = sprite.BufferPixels[rhsBufferIndex];
                }
            }
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public BufferPixelType[] BufferPixels { get; }
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
            BufferPixelType bufferPixel
        )
        {
            if (Width <= x || 0 > x || Height <= y || 0 > y)
                return false;

            BufferPixels[x + y * Width] = bufferPixel;

            return true;
        }
        public bool SetPixel(
            int i,
            BufferPixelType bufferPixel
        )
        {
            if (i < 0 || i >= BufferPixels.Length)
                return false;

            BufferPixels[i] = bufferPixel;

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="charInfo"></param>
        public void Fill(BufferPixelType bufferPixel)
        {
            for (var i = 0; i < BufferPixels.Length; ++i)
            {
                BufferPixels[i] = bufferPixel;
            }
        }
    }
}
