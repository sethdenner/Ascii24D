using Engine.Render;
using Engine.Native;

namespace Engine.Characters
{
    public class CharacterFrameCounter : Character
    {
#if COLOR_MODE_4_BIT
        public CharacterFrameCounter(CHAR_INFO_ATTRIBUTE characterAttributes = CHAR_INFO_ATTRIBUTE.BG_WHITE | CHAR_INFO_ATTRIBUTE.FG_BLACK) : base()
        {
            _fps = 0;
            _frameTimes = new List<float>();
            _characterAttributes = characterAttributes;
        }
#elif COLOR_MODE_24_BIT
        /// <summary>
        /// 
        /// </summary>
        public CharacterFrameCounter() : base() 
        {
            _fps = 0;
            _frameTimes = new List<float>();
            _foregroundColor = new Native.ConsoleColor() { };
            _backgroundColor = new Native.ConsoleColor() {
                R = (byte)255, G = (byte)255, B = (byte)255
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="foregroundColor"></param>
        /// <param name="backgroundColor"></param>
        public CharacterFrameCounter(
            Native.ConsoleColor foregroundColor,
            Native.ConsoleColor backgroundColor
        ) : base() {
            _fps = 0;
            _frameTimes = new List<float>();
            _foregroundColor = foregroundColor;
            _backgroundColor = backgroundColor;
        }
#endif

        public void Update(float elapsedSeconds)
        {
            _frameTimes.Add(elapsedSeconds);

            float frameTimeSum = 0;
            for (int i = 0; i < _frameTimes.Count; ++i)
            {
                frameTimeSum += _frameTimes[i];
            }

            if (frameTimeSum > _updateEverySeconds)
            {
                float frameTimeAverage = frameTimeSum / _frameTimes.Count;
                _fps = Math.Floor(1.0f / frameTimeAverage);
                _frameTimes.Clear();
            }

        }

        public override void GenerateSprites()
        {
            string fpsString = _fps.ToString() + "FPS";

            Sprite sprite = new Sprite(
                fpsString.Length,
                1,
                -fpsString.Length,
                0
            );
            sprite.EdgeBehavior = EdgeBehavior.WRAP;
            for (int i = 0; i < fpsString.Length; ++i)
            {
                sprite.BufferPixels[i] = PixelManager.CreatePixel(
                    new Native.ConsoleColor() { },
                    new Native.ConsoleColor() {
                        R = (byte)200, G = (byte)200, B = (byte)200
                    },
                    (byte)fpsString[i],
                    (int)Math.Floor(Position.Z)
                );
            }
            Sprites.Clear();
            Sprites.Add(sprite);
        }
        private double _fps;
        private Native.ConsoleColor _foregroundColor;
        private Native.ConsoleColor _backgroundColor;
        private List<float> _frameTimes;
        private const float _updateEverySeconds = 1.0f;
    }
}
