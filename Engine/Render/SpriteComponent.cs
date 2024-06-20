using Engine.Native;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace Engine.Render {
    public struct SpriteComponent(
        int width,
        int height,
        Vector3 modelPosition,
        bool transformToScreenSpace,
        ConsolePixel[] bufferPixels
    ) {
        /// <summary>
        /// The total width of the sprite.
        /// </summary>
        public int Width = width;
        /// <summary>
        /// The total height of the sprite.
        /// </summary>
        public int Height = height;
        /// <summary>
        /// The position of this sprite in Model-Space (before world transform).
        /// </summary>
        public Vector3 ModelPosition = modelPosition;
        /// <summary>
        /// Flag used to determine if this sprite should be transformed to
        /// screen space. Should be false for UI or other elements that don't
        /// move with the camera.
        /// </summary>
        public bool TransformToScreenSpace = transformToScreenSpace;
          /// <summary>
        /// An array of <c>Pixel</c> instances representing the sprite.
        /// </summary>
        public ConsolePixel[] BufferPixels = bufferPixels;
  }
}
