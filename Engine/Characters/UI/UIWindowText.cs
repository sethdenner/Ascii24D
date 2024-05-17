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
            string windowText = ""
        ) : base(width, height, position)
        {
            Text = windowText;
            BackgroundColor = new Native.ConsoleColor() { };
            ForegroundColor = new Native.ConsoleColor() { 
                R = 255, G = 255, B = 255 
            };
        }
        /// <summary>
        /// 
        /// </summary>
        public override void GenerateSprites()
        {
            int windowPositionX = (int)Math.Floor(Position.X);
            int windowPositionY = (int)Math.Floor(Position.Y);
            base.GenerateSprites();
            Sprite textSprite = new(
                Width - PaddingRight - PaddingLeft - windowPositionX - BorderWidth,
                Height - PaddingTop - PaddingBottom - windowPositionY - BorderWidth,
                PaddingLeft + windowPositionX + BorderWidth,
                PaddingTop + windowPositionY + BorderWidth
            );
            textSprite.Fill(PixelManager.CreatePixel(
                ForegroundColor,
                BackgroundColor,
                (byte)' ',
                (int)Math.Floor(Position.Z) + 1
            ));

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
                    PixelManager.CreatePixel(
                        ForegroundColor,
                        BackgroundColor,
                        (byte)Text[i],
                        (int)Math.Floor(Position.Z) + 1
                    )
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
        public Native.ConsoleColor ForegroundColor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Native.ConsoleColor BackgroundColor { get; set; }
    }
}
