using Engine.Core;
using System.Numerics;

namespace Engine.Characters {
    public class WorldTransformMessage(
        int entityID,
        Matrix4x4 world
    ) : Message {
        delegate void WorldTransformDelegate(int entityID, Matrix4x4 world);

        public int EntityID = entityID;
        public Matrix4x4 World = world;

        public override void Send() {
            Messenger<WorldTransformDelegate>.Trigger?.Invoke(EntityID, World);
        }
    }
}
