using Engine.Native;

namespace Engine.Render
{
    /// <summary>
    /// Enumeration used for specifying how to handle border overflow during
    /// <c>Sprite</c> addition.
    /// </summary>
    public enum EdgeBehavior
    {
        CLAMP = 0,
        WRAP = 1
    }
    /// <summary>
    /// A <c>Sprite</c> is a collection of BufferPixel objects meant to be
    /// rendered to a console buffer as a square block of text and attributes.
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
        public static Sprite<BufferPixelType> operator +(
            Sprite<BufferPixelType> lhs,
            Sprite<BufferPixelType> rhs
        )
        {
            for (int y = 0; y < rhs.Height; ++y)
            {
                for (int x = 0; x < rhs.Width; ++x)
                {
                    int xCoord = x + rhs.OffsetX;
                    int yCoord = y + rhs.OffsetY;
                    if (
                        EdgeBehavior.CLAMP ==
                        lhs.EdgeBehavior
                    )
                    {
                        // Handle clamping.
                        if (xCoord >= lhs.Width || xCoord < 0) continue;
                        if (yCoord >= lhs.Height || yCoord < 0) continue;
                    }
                    else if (
                        EdgeBehavior.WRAP ==
                        lhs.EdgeBehavior
                    )
                    {
                        // Handle wrapping.
                        if (xCoord >= lhs.Width || xCoord < 0) xCoord = (
                            lhs.Width - Math.Abs(xCoord) % lhs.Width
                        );
                        if (yCoord >= lhs.Height || yCoord < 0) yCoord = (
                            lhs.Height - Math.Abs(yCoord) % lhs.Height
                        );
                    }
                    int lhsBufferIndex = xCoord + yCoord * lhs.Width;
                    int rhsBufferIndex = x + y * rhs.Width;
                    lhs.BufferPixels[lhsBufferIndex] = rhs.BufferPixels[rhsBufferIndex];
                }
            }
            return lhs;
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
    /// <summary>
    /// 
    /// </summary>
    public class Sprite : Sprite<ConsolePixel>
    {
        /// <summary>
        /// <c>Sprite</c> default constructor.
        /// </summary>
        public Sprite() : base()
        { }
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
        ) : base(
            width,
            height,
            offsetX,
            offsetY,
            edgeBehavior
        ) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Sprite operator +(
            Sprite lhs,
            Sprite rhs
        )
        {
            return (Sprite)(
                (Sprite<ConsolePixel>)lhs + (Sprite<ConsolePixel>)rhs
            );
        }
    }
}
