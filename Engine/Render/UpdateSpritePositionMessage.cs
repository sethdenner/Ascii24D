using Engine.Core;
using System.Numerics;

namespace Engine.Render {
    public class UpdateSpritePositionMessage(
        int entityID,
        Vector3 position
    ) : Message {
        public delegate void Delegate(
            int entityID,
            Vector3 position
        );
        public int EntityID = entityID;
        public Vector3 Position = position;
        public override void Send() {
            Messenger<Delegate>.Trigger?.Invoke(
                EntityID,
                Position
            );
        }
    }
}
