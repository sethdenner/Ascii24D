using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Stamp is a collection of BufferPixel objects meant to be
    /// rendered to a FrameBuffer as a square.
    /// </summary>
    public class Sprite
    {
        public Sprite(int width, int height, int offsetX = 0, int offsetY = 0)
        {
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
            BufferPixels = new CHAR_INFO[width * height];
        }
        public static Sprite operator +(Sprite lhs, Sprite rhs)
        {
            for (int y = 0; y < rhs.Height; ++y)
            {
                for (int x = 0; x < rhs.Width; ++x)
                {
                    int xCoord = x + rhs.OffsetX;
                    int yCoord = y + rhs.OffsetY;
                    // Handle clamping.
                    if (xCoord >= lhs.Width || xCoord < 0) continue;
                    if (yCoord >= lhs.Height || yCoord < 0) continue;

                    // Handle wrapping.
                    if (xCoord >= lhs.Width || xCoord < 0) xCoord = lhs.Width - Math.Abs(xCoord) % lhs.Width;
                    if (yCoord >= lhs.Height || yCoord < 0) yCoord = lhs.Height - Math.Abs(yCoord) % lhs.Height;

                    int lhsBufferIndex = xCoord + yCoord * lhs.Width;
                    int rhsBufferIndex = x + y * rhs.Width;
                    lhs.BufferPixels[lhsBufferIndex] = rhs.BufferPixels[rhsBufferIndex];
                    
                }
            }
            return lhs;
        }
        public CHAR_INFO[] BufferPixels { get; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int OffsetX { get; set; }

        public int OffsetY { get; set; }

        public bool SetPixel(int x, int y, char character, CHAR_INFO_ATTRIBUTE charAttributes)
        {
            if (Width <= x || 0 > x || Height <= y || 0 > y)
                return false;

            BufferPixels[x + y * Width] = new CHAR_INFO() {
                Char = character,
                Attributes = (ushort)charAttributes
            };

            return true;
        }
    }
}
