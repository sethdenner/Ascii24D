using Engine.Core;
using Engine.Native;

namespace Engine.Render {
    public class UpdateFramebufferMessage(
        int entityID,
        ConsolePixel[] bufferPixels
    ) : Message {
        public delegate void Delegate(int entityID, ConsolePixel[] bufferPixels);
        public int EntityID = entityID;
        public ConsolePixel[] BufferPixels = bufferPixels;
        public override void Send() {
            Messenger<Delegate>.Trigger?.Invoke(
                EntityID,
                BufferPixels
            );
        }
    }
}
