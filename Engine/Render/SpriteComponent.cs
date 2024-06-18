using Engine.Native;
using System.Numerics;

namespace Engine.Render {
    public struct SpriteComponent {
        /// <summary>
        /// An array of <c>Pixel</c> instances representing the sprite.
        /// </summary>
        public ConsolePixel[] BufferPixels;
        /// <summary>
        /// The total width of the sprite.
        /// </summary>
        public int Width;
        /// <summary>
        /// The total height of the sprite.
        /// </summary>
        public int Height;
        /// <summary>
        /// The position of this sprite in Model-Space (before world transform).
        /// </summary>
        public Vector3 ModelPosition;
        public bool TransformToScreenSpace;
    }
}
