using System.Numerics;
using Engine.Native;
using Engine.Render;

namespace Engine.Characters.UI
{
    /// <summary>
    /// The <c>ChildLayout</c> enum corresponds to wether
    /// relatively positioned children should be arranged
    /// vertically or horizontally.
    /// </summary>
    public enum ChildLayout
    {
        Vertical,
        Horizontal
    }
    /// <summary>
    /// <c>UIWindow</c> is a <c>UI</c> derived class that
    /// represents a user interface window. Windows can be
    /// nested and automatically sized and positioned according
    /// to a variety of parameters.
    /// </summary>
    public class UIWindow : UI
    {
        public UIWindow() : base()
        {
            Width = 0;
            Height = 0;
            Position = new Vector3();
            BackgroundPixel = new ConsolePixel();
            BorderPixel = new ConsolePixel();
            BorderWidth = 0;
            ShowBorder = false;
            PaddingTop = 0;
            PaddingBottom = 0;
            PaddingLeft = 0;
            PaddingRight = 0;
            Layout = ChildLayout.Horizontal;
        }
        /// <summary>
        /// <c>UIWindow</c> constructor. Intializes the window width and
        /// height as well as the character attributes for the background
        /// and the borders. <c>position</c> is relative to the parent
        /// <c>UIWindow</c>.
        /// </summary>
        /// <param name="width">The width of the user interface in screen pixels.</param>
        /// <param name="height">The height of the user interface in screen pixels.</param>
        /// <param name="position">
        /// The position of the user interface relative to the top left corner of the parent
        /// <c>UIWindow</c> or to the to left corner of the screen in screen pixels. 
        /// </param>
        public UIWindow(
            int width,
            int height,
            Vector3 position,
            ConsolePixel backgroundPixel,
            ConsolePixel borderPixel
        ) : base() 
        {
            int depth = (int)Math.Floor(position.Z);
            Width = width;
            Height = height;
            Position = position;
            BackgroundPixel = backgroundPixel;
            BorderPixel = borderPixel; ;
            BorderWidth = 1;
            ShowBorder = true;
            PaddingTop = 0;
            PaddingBottom = 0;
            PaddingLeft = 0;
            PaddingRight = 0;
            Layout = ChildLayout.Horizontal;
        }
        /// <summary>
        /// <c>UIWindow</c> constructor. Intializes the window width and
        /// height as well as the character attributes for the background
        /// and the borders. <c>position</c> is relative to the parent. 24bit
        /// color version. Build with symbol <c>COLOR_MODE_24_BIT</c> defined to
        /// enable.
        /// </summary>
        /// <param name="width">The width of the user interface in screen pixels.</param>
        /// <param name="height">The height of the user interface in screen pixels.</param>
        /// <param name="position">
        /// The position of the user interface relative to the top left corner of the parent
        /// <c>UIWindow</c> or to the to left corner of the screen in screen pixels. 
        /// </param>
        /// <param name="backgroundPixel">
        /// <c>ConsolePixel</c> instance that will be used for background pixels
        /// in the window.
        /// </param>
        /// <param name="borderPixel">
        /// <c>ConsolePixel</c> instance that will be used for border pixels in
        /// the window.
        /// </param>
        /// <param name="borderWidth">
        /// The width of the window border in screen pixels. Borders are rendered inside
        /// the window dimensions.
        /// </param>
        /// <param name="paddingBottom">
        /// Space reserved at the bottom of the window when automatically sizing and
        /// positioning child user interface elements.
        /// </param>
        /// <param name="paddingLeft">
        /// Space reserved at the left side of the window when automatically sizing and
        /// positioning child user interface elements.
        /// </param>
        /// <param name="paddingRight">
        /// Space reserved at the right side of the window when automatically sizing and
        /// positioning child user interface elements.
        /// </param>
        /// <param name="paddingTop">
        /// </param>
        /// Space reserved at the top of the window when automatically sizing and
        /// positioning child user interface elements.
        /// <param name="showBorder">
        /// Boolean value indicating if the window border should be drawn.
        /// </param>
        public UIWindow(
            int width,
            int height,
            Vector3 position,
            ConsolePixel backgroundPixel,
            ConsolePixel borderPixel,
            int borderWidth,
            int paddingBottom,
            int paddingLeft,
            int paddingRight,
            int paddingTop,
            bool showBorder
        )
        {
            Width = width;
            Height = height;
            Position = position;
            BackgroundPixel = backgroundPixel;
            BorderPixel = borderPixel;
            BorderWidth = borderWidth;
            PaddingBottom = paddingBottom;
            PaddingTop = paddingTop;
            PaddingLeft = paddingLeft;
            PaddingRight = paddingRight;
            ShowBorder = showBorder;
            Layout = ChildLayout.Horizontal;
       }
        /// <summary>
        /// <c>InitializeUIWindow</c> is a method that initializes all the
        /// necessary properties associated with a <c>UIWindow</c>. 24bit color
        /// version. Build with <c>COLOR_MODE_24_BIT</c> symbol edfined to
        /// enable.
        /// <c>UIWindow</c>.
        /// </summary>
        /// <param name="width">The width of the user interface in screen pixels.</param>
        /// <param name="height">The height of the user interface in screen pixels.</param>
        /// <param name="position">
        /// The position of the user interface relative to the top left corner of the parent
        /// <c>UIWindow</c> or to the to left corner of the screen in screen pixels. 
        /// </param>
        /// <param name="backgroundPixel">
        /// A <c>ConsolePixel</c> instance that will be used to fill the
        /// background of the window.
        /// </param>
        /// <param name="borderPixel">
        /// A <c>ConsolePixel</c> instance that will be used for the border of
        /// the window.
        /// </param>
        /// <param name="borderWidth">
        /// The width of the window border in screen pixels. Borders are rendered inside
        /// the window dimensions.
        /// </param>
        /// <param name="paddingBottom">
        /// Space reserved at the bottom of the window when automatically sizing and
        /// positioning child user interface elements.
        /// </param>
        /// <param name="paddingLeft">
        /// Space reserved at the left side of the window when automatically sizing and
        /// positioning child user interface elements.
        /// </param>
        /// <param name="paddingRight">
        /// Space reserved at the right side of the window when automatically sizing and
        /// positioning child user interface elements.
        /// </param>
        /// <param name="paddingTop">
        /// </param>
        /// Space reserved at the top of the window when automatically sizing and
        /// positioning child user interface elements.
        /// <param name="showBorder">
        /// Boolean value indicating if the window border should be drawn.
        /// </param>
        private void InitializeUIWindow(
            int width,
            int height,
            Vector3 position,
            ConsolePixel backgroundPixel,
            ConsolePixel borderPixel,
            int borderWidth,
            int paddingBottom = 0,
            int paddingLeft = 0,
            int paddingRight = 0,
            int paddingTop = 0,
            bool showBorder = true
        )
        {
            Width = width;
            Height = height;
            Position = position;
            BackgroundPixel = backgroundPixel;
            BorderPixel = borderPixel;
            BorderWidth = borderWidth;
            ShowBorder = showBorder;
            PaddingTop = paddingTop;
            PaddingBottom = paddingBottom;
            PaddingLeft = paddingLeft;
            PaddingRight = paddingRight;
            Layout = ChildLayout.Horizontal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool IsPixelAtIndexBorder(int i) {
            // Test if the pixel at index i is at the edge of the sprite.
            return (
                i % Width < BorderWidth || // Left
                i % Width > Width - BorderWidth - 1 || // Right
                i < Width * BorderWidth || // Top
                i > Width * Height - Width * BorderWidth - 1 // Bottom
            );
        }
        /// <summary>
        /// 
        /// </summary>
        public override void GenerateSprites()
        {
            Sprite windowSprite = new Sprite(
                Width,
                Height,
                (int)Math.Floor(Position.X),
                (int)Math.Floor(Position.Y)
            );
            for (int i = 0; i < windowSprite.BufferPixels.Length; ++i)
            {
                if (IsPixelAtIndexBorder(i)) {
                    // This is a border pixel.
                    windowSprite.BufferPixels[i] = BorderPixel;
                } else {
                    windowSprite.BufferPixels[i] = BackgroundPixel;
                }
            }

            _sprites = [windowSprite];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        public void AddChildWindow(UIWindow window)
        {
            if (0 == window.Width && 0 == window.Height)
            {
                window.Width = 
                    Width - (2 * BorderWidth) -
                    PaddingLeft - PaddingRight;
                window.Height = Height - (2 * BorderWidth) -
                    PaddingTop - PaddingBottom;
            }

            window.Position = new Vector3(
                window.Position.X + BorderWidth,
                window.Position.Y + BorderWidth,
                (int)Math.Floor(Position.Z + 1)
            );

            AddChild(window);
        }
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
        public int PaddingTop { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PaddingRight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PaddingBottom { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PaddingLeft { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ChildLayout Layout { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int BorderWidth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ShowBorder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ConsolePixel BackgroundPixel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ConsolePixel BorderPixel { get; set; }
    }
}
