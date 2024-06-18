using Engine.Native;
using System.Numerics;

namespace Engine.Characters.UI {
    public struct UIProceduralSpritesComponent {
        public int SpriteEntityID;
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// 
        /// </summary>
        public int Width;
        /// <summary>
        /// 
        /// </summary>
        public int Height;
        /// <summary>
        /// 
        /// </summary>
        public int PaddingTop;
        /// <summary>
        /// 
        /// </summary>
        public int PaddingRight;
        /// <summary>
        /// 
        /// </summary>
        public int PaddingBottom;
        /// <summary>
        /// 
        /// </summary>
        public int PaddingLeft;
        /// <summary>
        /// 
        /// </summary>
        public ChildLayout Layout;
        /// <summary>
        /// 
        /// </summary>
        public int BorderWidth;
        /// <summary>
        /// 
        /// </summary>
        public bool ShowBorder;
        /// <summary>
        /// 
        /// </summary>
        public ConsolePixel BackgroundPixel;
        /// <summary>
        /// 
        /// </summary>
        public ConsolePixel BorderPixel;
        /// <summary>
        /// 
        /// </summary>
        public byte TextForegroundColorIndex;
        /// <summary>
        /// 
        /// </summary>
        public byte TextBackgroundColorIndex;
        /// <summary>
        /// 
        /// </summary>
        public string Text;
    }
}
