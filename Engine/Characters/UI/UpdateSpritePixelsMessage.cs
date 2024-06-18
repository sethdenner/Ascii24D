using Engine.Core;
using Engine.Native;

namespace Engine.Characters.UI {
    public class UpdateSpritePixelsMessage(
        int entityID,
        ConsolePixel[] bufferPixels
    ) : Message {
        delegate void UpdateSpritePixelsMessageDelegate(
            int entityID,
            ConsolePixel[] bufferPixels
        );

        public int EntityID = entityID;
        public ConsolePixel[] BufferPixels = bufferPixels;
        public override void Send() {
            Messenger<UpdateSpritePixelsMessageDelegate>.Trigger?.Invoke(
                EntityID,
                BufferPixels
            );
        }
    }
}
