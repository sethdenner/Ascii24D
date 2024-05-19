using Engine.Render;
using Engine.Native;

namespace Engine.Characters
{
    public class CharacterFrameCounter : Character
    {
        /// <summary>
        /// 
        /// </summary>
        public byte ForegroundColorIndex {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public byte BackgroundColorIndex {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public CharacterFrameCounter() : base() 
        {
            _fps = 0;
            _frameTimes = new List<float>();
            ForegroundColorIndex = 0;
            BackgroundColorIndex = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="foregroundColorIndex"></param>
        /// <param name="backgroundColorIndex"></param>
        public CharacterFrameCounter(
            byte foregroundColorIndex,
            byte backgroundColorIndex
        ) : base() {
            _fps = 0;
            _frameTimes = new List<float>();
            ForegroundColorIndex = foregroundColorIndex;
            BackgroundColorIndex = backgroundColorIndex;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elapsedSeconds"></param>
        public override void Update(float elapsedSeconds)
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
        /// <summary>
        /// 
        /// </summary>
        public override void GenerateSprites()
        {
            Sprite sprite = new(
                FpsString.Length,
                1,
                (int)Math.Floor(Position.X),
                (int)Math.Floor(Position.Y)
            );
            for (int i = 0; i < FpsString.Length; ++i)
            {
                sprite.BufferPixels[i] = new ConsolePixel {
                    ForegroundColorIndex = ForegroundColorIndex,
                    BackgroundColorIndex = BackgroundColorIndex,
                    CharacterCode = (byte)FpsString[i]
                };
            }
            Sprites.Clear();
            Sprites.Add(sprite);
        }
        /// <summary>
        /// 
        /// </summary>
        public string FpsString {
            get {
                return _fps.ToString() + "FPS";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private double _fps;
        /// <summary>
        /// 
        /// </summary>
        private List<float> _frameTimes;
        /// <summary>
        /// 
        /// </summary>
        private const float _updateEverySeconds = 1.0f;
    }
}
