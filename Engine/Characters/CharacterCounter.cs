using Engine.Render;
using Engine.Native;

namespace Engine.Characters
{
    /// <summary>
    /// A simple character that keeps track of and
    /// renders an integer that can be incremented
    /// or decremented.
    /// </summary>
    public class CharacterCounter : Character
    {
        /// <summary>
        /// <c>CharacterCounter</c> default constructor.
        /// Initializes the <c>_count</c> to 0.
        /// </summary>
        public CharacterCounter() : base()
        {
            _count = 0;
        }
        /// <summary>
        /// <c>Increment</c> increments the <c>_count/c>
        /// member. 1 is the default but can add any amount.
        /// </summary>
        /// <param name="amount">The amount to increment the counter.</param>
        public void Increment(int amount = 1)
        {
            _count += amount;
        }
        /// <summary>
        /// <c>Decrement</c> decrements the <c>_count/c>
        /// member. 1 is the default but can subtract any
        /// amount.
        /// </summary>
        /// <param name="amount">The amount to decrement the counter.</param>
        public void Decrement(int amount = 1)
        {
            _count -= amount;
        }
        /// <summary>
        /// <c>GenerateSprites</c> override. Generate the counter.
        /// Render each character in the counter string to a sprite.
        /// </summary>
        public override void GenerateSprites()
        {
            // Convert the counter to a string.
            string countString = _count.ToString();
            // Intitialize a sprite with the proper dimensions.
            Sprite sprite = new Sprite(countString.Length, 1);
            // Iterate over the counter string.
            for (int i = 0; i < countString.Length; ++i)
            {
                sprite.SetPixel(i, PixelManager.CreatePixel(
                    new Native.ConsoleColor() { },
                    new Native.ConsoleColor() {
                        R = (byte)255, G = (byte)255, B = (byte)255
                    },
                    (byte)countString[i],
                    0
                ));
            }
            // Clear any previous sprites.
            Sprites.Clear();
            // Add the newly generated sprite.
            Sprites.Add(sprite);
        }
        /// <summary>
        /// 
        /// </summary>
        public int Count { get { return _count; } set { _count = value; } }
        /// <summary>
        /// 
        /// </summary>
        public Native.ConsoleColor ForegroundColor { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public Native.ConsoleColor BackgroundColor { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        private int _count;
    }
}
