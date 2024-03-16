using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Characters.UI
{
    public class UIWindowText : UIWindow
    {
        public UIWindowText(int width, int height, Vector2 position, string windowText = "") : base(width, height, position)
        {
            Text = windowText;
            CharAttributes = CHAR_INFO_ATTRIBUTE.FG_WHITE | CHAR_INFO_ATTRIBUTE.BG_BLACK;
        }

        public override void GenerateSprites()
        {
            int windowPositionX = (int)Math.Floor(Position.X);
            int windowPositionY = (int)Math.Floor(Position.Y);
            base.GenerateSprites();
            Sprite textSprite = new Sprite(
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
                textSprite.SetPixel(x, y, Text[i], CharAttributes);
            }

            Sprites.Add(textSprite);
        }

        public override Sprite Render()
        { 
            Sprite baseSprite = base.Render();
            return baseSprite;
        }



        public string Text { get; set; }
        public CHAR_INFO_ATTRIBUTE CharAttributes { get; set; }
    }
}
