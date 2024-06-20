using Engine.Native;
using Engine.Render;
using System.Numerics;

namespace Engine.Characters.UI {
    public struct UIProceduralSpritesComponent(
        int spriteEntityId,
        int width,
        int height,
        Vector3 position,
        ConsolePixel backgroundPixel,
        ConsolePixel borderPixel,
        int borderWidth,
        int paddingTop,
        int paddingRight,
        int paddingBottom,
        int paddingLeft,
        string text,
        byte textForegroundColor,
        byte textBackgroundColor,
        ChildLayout childLayout,
        bool autoSize,
        int autoSizeMargin = 0
    ) {
        public int SpriteEntityID = spriteEntityId;
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Position = position;
        /// <summary>
        /// 
        /// </summary>
        public int Width = width;
        /// <summary>
        /// 
        /// </summary>
        public int Height = height;
        /// <summary>
        /// 
        /// </summary>
        public int PaddingTop = paddingTop;
        /// <summary>
        /// 
        /// </summary>
        public int PaddingRight = paddingRight;
        /// <summary>
        /// 
        /// </summary>
        public int PaddingBottom = paddingBottom;
        /// <summary>
        /// 
        /// </summary>
        public int PaddingLeft = paddingLeft;
        /// <summary>
        /// 
        /// </summary>
        public ChildLayout Layout = childLayout;
        /// <summary>
        /// 
        /// </summary>
        public int BorderWidth = borderWidth;
        /// <summary>
        /// 
        /// </summary>
        public ConsolePixel BackgroundPixel = backgroundPixel;
        /// <summary>
        /// 
        /// </summary>
        public ConsolePixel BorderPixel = borderPixel;
        /// <summary>
        /// 
        /// </summary>
        public byte TextForegroundColorIndex = textForegroundColor;
        /// <summary>
        /// 
        /// </summary>
        public byte TextBackgroundColorIndex = textBackgroundColor;
        /// <summary>
        /// 
        /// </summary>
        public string Text = text;
        /// <summary>
        /// 
        /// </summary>
        public bool Regenerate = false;
        /// <summary>
        /// 
        /// </summary>
        public bool AutoSize = autoSize;
        /// <summary>
        /// 
        /// </summary>
        public int AutoSizeMargin = autoSizeMargin;
    }
}
