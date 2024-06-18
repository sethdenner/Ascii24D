using Engine.Core.ECS;
using Engine.Native;

namespace Engine.Characters.UI {
    public class UIProceduralSpritesSystem :
        System<UIProceduralSpritesComponent> {
        public override void Cleanup() {
            throw new NotImplementedException();
        }

        public override void SetupComponent(
            ref UIProceduralSpritesComponent component
        ) {
            GenerateUISprites(ref component);
        }

        public override void UpdateComponent(
            ref UIProceduralSpritesComponent component,
            long step,
            bool headless = false
        ) {
        }
        public void GenerateUISprites(
            ref UIProceduralSpritesComponent component
        ) {
            ConsolePixel[] windowSprite = new ConsolePixel[
                component.Width * component.Height
            ];
            int windowOffsetX = (int)Math.Ceiling(component.Position.X);
            int windowOffsetY = (int)Math.Ceiling(component.Position.Y);

            for (int i = 0; i < windowSprite.Length; ++i) {
                if (IsPixelAtIndexBorder(i, component)) {
                    // This is a border pixel.
                    windowSprite[i] = component.BorderPixel;
                } else {
                    windowSprite[i] = component.BackgroundPixel;
                }
            }

            int positionX = (int)Math.Ceiling(component.Position.X);
            int positionY = (int)Math.Ceiling(component.Position.Y);
            int width = (
                component.Width - component.PaddingRight -
                component.PaddingLeft - (2 * component.BorderWidth)
            );
            int height = (
                component.Height - component.PaddingTop -
                component.PaddingBottom - (2 * component.BorderWidth)
            );

            if (width <= 0 || height <= 0) {
                return;
            }
            ConsolePixel[] textSprite = new ConsolePixel[width * height];
            int offsetX = component.PaddingLeft + component.BorderWidth + positionX;
            int offsetY = component.PaddingTop + component.BorderWidth + positionY;

            int newLineCount = 0;
            int newLineIndexOffset = 0;
            int usableWidth = (
                component.Width - component.PaddingLeft -
                component.PaddingRight -
                2 * component.BorderWidth
            );
            int usableHeight = (
                component.Height - component.PaddingTop -
                component.PaddingBottom -
                2 * component.BorderWidth
            );
            for (int i = 0; i < component.Text.Length; ++i) {
                int adjustedIndex = i - newLineIndexOffset;
                int x = adjustedIndex % usableWidth;
                int y = adjustedIndex / usableWidth + newLineCount;
                if ('\n' == component.Text[i]) {
                    if (0 != x) {
                        ++newLineCount;
                        newLineIndexOffset += x + 1;
                    } else {
                        ++newLineIndexOffset;
                    }
                    continue;
                }

                SetPixel(
                    textSprite,
                    usableWidth,
                    usableHeight,
                    x,
                    y,
                    new ConsolePixel(
                        component.TextForegroundColorIndex,
                        component.TextBackgroundColorIndex,
                        component.Text[i]
                    )
                );
            }

        }
        public bool IsPixelAtIndexBorder(
            int i,
            UIProceduralSpritesComponent component
        ) {
            // Test if the pixel at index i is at the edge of the sprite.
            return (
                i % component.Width < component.BorderWidth || // Left
                i % component.Width > component.Width -
                    component.BorderWidth - 1 || // Right
                i < component.Width * component.BorderWidth || // Top
                i > component.Width * component.Height -
                    component.Width * component.BorderWidth - 1 // Bottom
            );
        }
        /// <summary>
        /// <c>SetPixel</c> method sets the pixel in the pixel buffer at
        /// position (x, y) to the provided <c>Pixel</c> instance.
        /// </summary>
        /// <param name="pixels">
        /// A span pointing to the pixel buffer to write to.
        /// </param>
        /// <param name="width">
        /// The width of the pixel buffer.
        /// </param>
        /// <param name="height"></param>
        /// The height of the pixel buffer.
        /// <param name="x">
        /// The position on the <c>Sprite</c> in the x axis.
        /// </param>
        /// <param name="y">
        /// The position on the <c>Sprite</c> in the y axis.
        /// </param>
        /// <param name="bufferPixel">
        /// An instance of <c>Pixel</c> that will be copied to the pixel buffer
        /// at the specified location.
        /// </param>
        /// <returns>
        /// <c>true</c> if the pixel was set. <c>false</c> if the pixel was
        /// rejected for some reason.
        /// </returns>
        public bool SetPixel(
            Span<ConsolePixel> pixels,
            int width,
            int height,
            int x,
            int y,
            ConsolePixel bufferPixel
        ) {
            if (width <= x || 0 > x || height <= y || 0 > y)
                return false;

            pixels[x + y * width] = bufferPixel;

            return true;
        }
    }
}
