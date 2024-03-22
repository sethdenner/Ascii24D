using Engine.Native;

namespace Engine.Render
{
    /// <summary>
    /// 
    /// </summary>
    public static class PixelManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        static  PixelManager()
        {
            Pixels = new Pixel[10000];
            PixelMap = new Dictionary<int, int>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="foregroundColor"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static Pixel CreatePixel(
            Native.ConsoleColor foregroundColor,
            Native.ConsoleColor backgroundColor,
            byte character,
            int depth
        )
        {
            // I don't like that depth is part of the key here but we're just
            // going to roll with it for now. We will come back and optimize
            // this if needed (so probably never).
            int key = foregroundColor.R + foregroundColor.G +
                foregroundColor.B + backgroundColor.R + backgroundColor.G +
                backgroundColor.B + character + depth;

            if (!PixelMap.ContainsKey(key))
            {
                Pixels[PixelCount] = new Pixel(
                    foregroundColor,
                    backgroundColor,
                    character,
                    depth
                );
                PixelMap[key] = PixelCount;
                ++PixelCount;
            }

            return Pixels[PixelMap[key]];
        }
        /// <summary>
        /// 
        /// </summary>
        public static int PixelCount { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<int, int> PixelMap { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static Pixel[] Pixels { get; set; }
    }
}
