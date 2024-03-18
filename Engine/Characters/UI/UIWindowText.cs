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
            CharAttributes = CHAR_INFO_ATTRIBUTE.FG_WHITE | CHAR_INFO_ATTRIBUTE.BG_BLACK;
        }
        /// <summary>
        /// 
        /// </summary>
        public override void GenerateSprites()
        {
            int windowPositionX = (int)Math.Floor(Position.X);
            int windowPositionY = (int)Math.Floor(Position.Y);
            base.GenerateSprites();
            Sprite<CHAR_INFO> textSprite = new Sprite<CHAR_INFO>(
                Width - PaddingRight - PaddingLeft - windowPositionX - BorderWidth,
                Height - PaddingTop - PaddingBottom - windowPositionY - BorderWidth,
                PaddingLeft + windowPositionX,
                PaddingTop + windowPositionY
            );

            int newLineCount = 0;
            int newLineIndexOffset = 0;
            int usableWidth = Width - PaddingLeft - PaddingRight - 2 * BorderWidth;
            int usableHeight = Height - PaddingTop - PaddingBottom - 2 * BorderWidth;
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
                    new CHAR_INFO() {
                        Char = Text[i],
                        Attributes = (ushort)CharAttributes
                    });
            }

            Sprites.Add(textSprite);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Sprite<CHAR_INFO> Render()
        { 
            Sprite<CHAR_INFO> baseSprite = base.Render();
            return baseSprite;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CHAR_INFO_ATTRIBUTE CharAttributes { get; set; }
    }
}
