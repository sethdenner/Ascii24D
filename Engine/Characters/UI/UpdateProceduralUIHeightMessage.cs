using Engine.Core;

namespace Engine.Characters.UI {
    public class UpdateProceduralUIHeightMessage(
        int entityID,
        int height
    ) : Message {
        public delegate void Delegate(
            int entityID,
            int height
        );

        public int EntityID = entityID;
        public int Height = height;
        public override void Send() {
            Messenger<Delegate>.Trigger?.Invoke(
                EntityID,
                Height
            );
        }
    }
}
