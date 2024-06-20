using Engine.Core;
using Engine.Native;

namespace Engine.Characters.UI {
    public class UpdateSpritePixelsMessage(
        int entityID,
        int width,
        int height,
        ConsolePixel[] bufferPixels
    ) : Message {
        public delegate void Delegate(
            int entityID,
            int width,
            int height,
            ConsolePixel[] bufferPixels
        );

        public int EntityID = entityID;
        public int Width = width;
        public int Height = height;
        public ConsolePixel[] BufferPixels = bufferPixels;
        public override void Send() {
            Messenger<Delegate>.Trigger?.Invoke(
                EntityID,
                Width,
                Height,
                BufferPixels
            );
        }
    }
}
