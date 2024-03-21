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
        }
        public Pixel(
            Native.ConsoleColor foregroundColor,
            Native.ConsoleColor backgroundColor,
            byte Character
        )
        {
            PixelDescription = new ConsolePixel()
            {
                ForegroundColor = foregroundColor,
                BackgroundColor = backgroundColor,
                CharacterCode = Character
            };
        }
        public ConsolePixel PixelDescription { get; set; }
    }
}
