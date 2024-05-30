using System.Numerics;
using Engine.Render;
using Engine.Native;

namespace Engine.Characters.UI
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="position"></param>
    /// <param name="windowText"></param>
    public class UIWindowText(
        int width,
        int height,
        Vector3 position,
        ConsolePixel backgroundPixel,
        ConsolePixel borderPixel,
        byte textForegroundColorIndex,
        byte textBackgroundColorIndex,
        string windowText = ""
        ) : UIWindow(width, height, position, backgroundPixel, borderPixel)
    {
        /// <summary>
        /// 
        /// </summary>
        public string Text {
            get; set;
        } = windowText;
        /// <summary>
        /// 
        /// </summary>
        public byte ForegroundColorIndex {
            get; set;
        } = textForegroundColorIndex;
        /// <summary>
        /// 
        /// </summary>
        public byte BackgroundColorIndex {
            get; set;
        } = textBackgroundColorIndex;

        /// <summary>
        /// 
        /// </summary>
        public override void GenerateSprites()
        {
            base.GenerateSprites();
            int positionX = (int)Math.Floor(Position.X);
            int positionY = (int)Math.Floor(Position.Y);
            int width = Width - PaddingRight - PaddingLeft - (2 * BorderWidth);
            int height = Height - PaddingTop - PaddingBottom - (2 * BorderWidth);
            if (width <= 0 || height <=0) {
                return;
            }
            Sprite textSprite = new(
                width,
                height,
                PaddingLeft + BorderWidth + positionX,
                PaddingTop + BorderWidth + positionY
            );

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
    }
}
