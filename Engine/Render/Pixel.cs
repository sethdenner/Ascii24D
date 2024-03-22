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
        public Pixel()
        {
            PixelDescription = new ConsolePixel() { };
            PixelDepth = 0;
        }
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
        public ConsolePixel PixelDescription { get; set; }
        public int PixelDepth { get; set; }
    }
}
