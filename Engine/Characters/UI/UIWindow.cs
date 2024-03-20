using System.Numerics;
using Engine.Render;
using Engine.Native;

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
        public UIWindow(int width, int height, Vector2 position) : base() 
        {
#if COLOR_MODE_4_BIT
            CHAR_INFO backgroundPixel = new CHAR_INFO()
            {
                Char = ' ',
                Attributes = CHAR_INFO_ATTRIBUTE.BG_WHITE | CHAR_INFO_ATTRIBUTE.FG_BLACK
            }
            CHAR_INFO borderPixel = new CHAR_INFO()
            {
                Char = '#',
                Attributes = CHAR_INFO_ATTRIBUTE.BG_DARK_GREY | CHAR_INFO_ATTRIBUTE.FG_GREY
            }
#elif COLOR_MODE_24_BIT
            ConsolePixel backgroundPixel = new ConsolePixel()
            {
                ForegroundColor = new Native.ConsoleColor() { },
                BackgroundColor = new Native.ConsoleColor() {
                    R = (byte)255, G = (byte)255, B = (byte)255
                },
                CharacterCode = (byte)' '
            };
            ConsolePixel borderPixel = new ConsolePixel()
            {
                ForegroundColor = new Native.ConsoleColor()
                {
                    R = (byte)125,
                    G = (byte)125,
                    B = (byte)125
                },
                BackgroundColor = new Native.ConsoleColor()
                {
                    R = (byte)50,
                    G = (byte)50,
                    B = (byte)50
                },
                CharacterCode = (byte)'#'
            };
#endif
            InitializeUIWindow(
                width,
                height,
                position,
                backgroundPixel,
                borderPixel,
                1
            );
        }
#if COLOR_MODE_4_BIT
        /// <summary>
        /// <c>UIWindow</c> constructor. Intializes the window width and
        /// height as well as the character attributes for the background
        /// and the borders. <c>position</c> is relative to the parent
        /// </summary>
        /// <param name="width">The width of the user interface in screen pixels.</param>
        /// <param name="height">The height of the user interface in screen pixels.</param>
        /// <param name="position">
        /// The position of the user interface relative to the top left corner of the parent
        /// <c>UIWindow</c> or to the to left corner of the screen in screen pixels. 
        /// </param>
        /// <param name="backgroundPixel">
        /// <c>CHAR_INFO</c> instance that will be used for background pixels in
        /// the window.
        /// </param>
        /// <param name="borderPixel">
        /// <c>CHAR_INFO</c> instance that will be used for border pixels in the
        /// window.
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
            Vector2 position,
            CHAR_INFO backgroundPixel,
            CHAR_INFO borderPixel,
            int borderWidth,
            int paddingBottom,
            int paddingLeft,
            int paddingRight,
            int paddingTop,
            bool showBorder
        ) 
        {
            InitializeUIWindow(
                width,
                height,
                position,
                backgroundPixel,
                borderPixel,
                borderWidth,
                paddingBottom,
                paddingLeft,
                paddingRight,
                paddingTop,
                showBorder
             );
        }
#elif COLOR_MODE_24_BIT
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
            Vector2 position,
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
            InitializeUIWindow(
                width,
                height,
                position,
                backgroundPixel,
                borderPixel,
                borderWidth,
                paddingBottom,
                paddingLeft,
                paddingRight,
                paddingTop,
                showBorder
             );
        }
#endif
#if COLOR_MODE_4_BIT
        /// <summary>
        /// <c>InitializeUIWindow</c> is a method that initializes all the
        /// necessary properties associated with a <c>UIWindow</c>.
        /// <c>UIWindow</c>.
        /// </summary>
        /// <param name="width">The width of the user interface in screen pixels.</param>
        /// <param name="height">The height of the user interface in screen pixels.</param>
        /// <param name="position">
        /// The position of the user interface relative to the top left corner of the parent
        /// <c>UIWindow</c> or to the to left corner of the screen in screen pixels. 
        /// </param>
        /// <param name="backgroundChar">
        /// The <c>char</c> value of the text character to be rendered as the background.
        /// </param>
        /// <param name="backgroundAttributes">
        /// <c>CHAR_INFOR_ATTRIBUTE</c> specifing the background color properties.
        /// </param>
        /// <param name="borderChar">
        /// The <c>char</c> value of the text character to be rendered as the border of
        /// the window.
        /// </param>
        /// <param name="borderAttributes">
        /// <c>CHAR_INFOR_ATTRIBUTE</c> specifing the border color properties.
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
            Vector2 position,
            CHAR_INFO backgroundPixel,
            CHAR_INFO foregroundPixel,
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
            BackgroundChar = backgroundChar;
            BackgroundAttributes = backgroundAttributes;
            BorderChar = borderChar;
            BorderAttributes = borderAttributes;
            BorderWidth = borderWidth;
            ShowBorder = showBorder;
            PaddingTop = paddingTop;
            PaddingBottom = paddingBottom;
            PaddingLeft = paddingLeft;
            PaddingRight = paddingRight;
            Layout = ChildLayout.Horizontal;
        }
#elif COLOR_MODE_24_BIT
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
            Vector2 position,
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
#endif
        /// <summary>
        /// 
        /// </summary>
        public override void GenerateSprites()
        {
            Sprite<ConsolePixel> windowSprite = new Sprite<ConsolePixel>(
                Width,
                Height
            );
            for (int i = 0; i < windowSprite.BufferPixels.Length; ++i)
            {
                if (
                    i % windowSprite.Width < BorderWidth || // Left edge.
                    i % windowSprite.Width > windowSprite.Width - BorderWidth - 1 || // Right edge
                    i < windowSprite.Width * BorderWidth || // Top edge
                    i > windowSprite.Width * windowSprite.Height - (windowSprite.Width * BorderWidth) - 1 // Bottom edge
                )
                {
                    // This is a border pixel.
                    windowSprite.BufferPixels[i] = BorderPixel;
                }
                else
                {
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
                window.Width = Width - (2 * BorderWidth) - PaddingLeft - PaddingRight;
                window.Height = Height - (2 * BorderWidth) - PaddingTop - PaddingBottom;
            }

            window.Position = new Vector2(
                window.Position.X + BorderWidth,
                window.Position.Y + BorderWidth
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
#if COLOR_MODE_4_BIT
        /// <summary>
        /// 
        /// </summary>
        public CHAR_INFO BorderPixel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CHAR_INFO BorderPixel { get; set; }
#elif COLOR_MODE_24_BIT
        public ConsolePixel BackgroundPixel { get; set; }
        public ConsolePixel BorderPixel { get; set; }
#endif
    }
}
