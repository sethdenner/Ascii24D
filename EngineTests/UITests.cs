using Engine.Characters.UI;
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
            Pixel backgroundPixel = new Pixel(
                new Engine.Native.ConsoleColor() { },
                new Engine.Native.ConsoleColor() { },
                (byte)'1',
                1
            );
            Pixel borderPixel = new Pixel(
                new Engine.Native.ConsoleColor() { },
                new Engine.Native.ConsoleColor() { },
                (byte)'2',
                2
            );
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
                position
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
