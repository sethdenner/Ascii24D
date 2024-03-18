using Engine.Render;
using Engine.Native;

namespace Engine.Characters
{
    public class CharacterFrameCounter : Character
    {
        public CharacterFrameCounter(CHAR_INFO_ATTRIBUTE characterAttributes = CHAR_INFO_ATTRIBUTE.BG_WHITE | CHAR_INFO_ATTRIBUTE.FG_BLACK) : base()
        {
            _fps = 0;
            _frameTimes = new List<float>();
            _characterAttributes = characterAttributes;
        }

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

            Sprite<CHAR_INFO> sprite = new Sprite<CHAR_INFO>(fpsString.Length, 1, -fpsString.Length, 0);
            for (int i = 0; i < fpsString.Length; ++i)
            {
                sprite.BufferPixels[i] = new CHAR_INFO() {
                    Char = fpsString[i],
                    Attributes = (ushort)_characterAttributes
                };
            }
            Sprites.Clear();
            Sprites.Add(sprite);
        }

        private double _fps;
        private CHAR_INFO_ATTRIBUTE _characterAttributes;
        private List<float> _frameTimes;
        private const float _updateEverySeconds = 1.0f;
    }
}
