using System.Numerics;
using Engine.Render;
using Engine.Native;

namespace Engine.Characters.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIWindowText : UIWindow
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="position"></param>
        /// <param name="windowText"></param>
        public UIWindowText(int width, int height, Vector2 position, string windowText = "") : base(width, height, position)
        {
            Text = windowText;
#if COLOR_MODE_4_BIT
            CharAttributes = CHAR_INFO_ATTRIBUTE.FG_WHITE | CHAR_INFO_ATTRIBUTE.BG_BLACK;
#elif COLOR_MODE_24_BIT
            BackgroundColor = new Native.ConsoleColor() { };
            ForegroundColor = new Native.ConsoleColor() { 
                R = 255, G = 255, B = 255 
            };
#endif
        }
        /// <summary>
        /// 
        /// </summary>
        public override void GenerateSprites()
        {
            int windowPositionX = (int)Math.Floor(Position.X);
            int windowPositionY = (int)Math.Floor(Position.Y);
            base.GenerateSprites();
            Sprite<ConsolePixel> textSprite = new Sprite<ConsolePixel>(
                Width - PaddingRight - PaddingLeft - windowPositionX - BorderWidth,
                Height - PaddingTop - PaddingBottom - windowPositionY - BorderWidth,
                PaddingLeft + windowPositionX,
                PaddingTop + windowPositionY
            );
            textSprite.Fill(new ConsolePixel() {
                ForegroundColor = ForegroundColor,
                BackgroundColor = BackgroundColor,
                CharacterCode = (byte)' '
            });

            int newLineCount = 0;
            int newLineIndexOffset = 0;
            int usableWidth = Width - PaddingLeft - PaddingRight - 2 * BorderWidth;
            for (int i = 0; i < Text.Length; ++i)
            {
                int adjustedIndex = i - newLineIndexOffset;
                int x = adjustedIndex % usableWidth;
                int y = adjustedIndex / usableWidth + newLineCount;
                if ('\n' == Text[i])
                {
                    if (0 != x)
                    {
                        ++newLineCount;
                        newLineIndexOffset += x + 1;
                    }
                    else
                    {
                        ++newLineIndexOffset;
                    }
                    continue;
                }

                textSprite.SetPixel(
                    x,
                    y,
                    new ConsolePixel() {
                        ForegroundColor = ForegroundColor,
                        BackgroundColor = BackgroundColor,
                        CharacterCode = (byte)Text[i]
                    }
                );
            }

            Sprites.Add(textSprite);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Sprite<ConsolePixel> Render()
        {
            Sprite<ConsolePixel> baseSprite = base.Render();
            return baseSprite;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Text { get; set; }
#if COLOR_MODE_4_BIT
        /// <summary>
        /// 
        /// </summary>
        public CHAR_INFO_ATTRIBUTE CharAttributes { get; set; }
#elif COLOR_MODE_24_BIT
        public Native.ConsoleColor ForegroundColor { get; set; }
        public Native.ConsoleColor BackgroundColor { get; set; }
#endif
    }
}
