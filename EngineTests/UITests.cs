using Engine.Characters.UI;
using Engine.Native;
using Engine.Render;
using System.Numerics;
using Xunit;

namespace EngineTests
{
    public class UITests
    {
        [Fact]
        public void TestUIWindowConstructors()
        {
            int width = 10; // arbitrary.
            int height = 20; //arbitrary.
            Vector3 position = new Vector3(
                1, 2, 3
            );
            ConsolePixel backgroundPixel = new() {
                ForegroundColorIndex = 0,
                BackgroundColorIndex = 0,
                CharacterCode = (byte)'1'
            };
            ConsolePixel borderPixel = new() {
                ForegroundColorIndex = 0,
                BackgroundColorIndex = 0,
                CharacterCode = (byte)'2'
            };
            int borderWidth = 1;
            int paddingBottom = 2;
            int paddingLeft = 3;
            int paddingRight = 4;
            int paddingTop = 5;
            bool showBorder = true;
            UIWindow window0 = new UIWindow();
            UIWindow window1 = new UIWindow(
                width,
                height,
                position,
                backgroundPixel,
                borderPixel
            );
            UIWindow window2 = new UIWindow(
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
    }
}
