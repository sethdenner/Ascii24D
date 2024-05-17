using Engine.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Render
{
    /// <summary>
    /// 
    /// </summary>
    public class Pixel
    {
        /// <summary>
        /// 
        /// </summary>
        public Pixel()
        {
            PixelDescription = new ConsolePixel() { };
            PixelDepth = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="foregroundColor"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="character"></param>
        /// <param name="depth"></param>
        public Pixel(
            Native.ConsoleColor foregroundColor,
            Native.ConsoleColor backgroundColor,
            byte character,
            int depth
        )
        {
            PixelDescription = new ConsolePixel()
            {
                ForegroundColor = foregroundColor,
                BackgroundColor = backgroundColor,
                CharacterCode = character
            };
            PixelDepth = depth;
        }
        /// <summary>
        /// 
        /// </summary>
        public ConsolePixel PixelDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PixelDepth { get; set; }
    }
}
