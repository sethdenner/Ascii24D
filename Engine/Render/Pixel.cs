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
        /// <param name="foregroundColorIndex"></param>
        /// <param name="backgroundColorIndex"></param>
        /// <param name="character"></param>
        /// <param name="depth"></param>
        public Pixel(
            byte foregroundColorIndex,
            byte backgroundColorIndex,
            byte character,
            int depth
        )
        {
            PixelDescription = new ConsolePixel()
            {
                ForegroundColorIndex = foregroundColorIndex,
                BackgroundColorIndex = backgroundColorIndex,
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
