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
        public UIWindowText(
            int width,
            int height,
            Vector3 position,
            ConsolePixel backgroundPixel,
            ConsolePixel borderPixel,
            byte textForegroundColorIndex,
            byte textBackgroundColorIndex,
            string windowText = ""
        ) : base(width, height, position, backgroundPixel, borderPixel)
        {
            Text = windowText;
            ForegroundColorIndex = textForegroundColorIndex;
            BackgroundColorIndex = textBackgroundColorIndex;
        }
        /// <summary>
        /// 
        /// </summary>
        public override void GenerateSprites()
        {
            base.GenerateSprites();
            int windowPositionX = (int)Math.Floor(Position.X);
            int windowPositionY = (int)Math.Floor(Position.Y);
            Sprite textSprite = new(
                Width - PaddingRight - PaddingLeft - windowPositionX - BorderWidth,
                Height - PaddingTop - PaddingBottom - windowPositionY - BorderWidth,
                PaddingLeft + windowPositionX + BorderWidth,
                PaddingTop + windowPositionY + BorderWidth
            );
            textSprite.Fill(new ConsolePixel() {
                ForegroundColorIndex = ForegroundColorIndex,
                BackgroundColorIndex = BackgroundColorIndex,
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
                        ForegroundColorIndex = ForegroundColorIndex,
                        BackgroundColorIndex = BackgroundColorIndex,
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
        public override void Render(Sprite renderTarget)
        {
            base.Render(renderTarget);
        }
        /// <summary>
        /// 
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public byte ForegroundColorIndex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public byte BackgroundColorIndex { get; set; }
    }
}
