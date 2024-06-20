using Engine.Core;

namespace Engine.Characters.UI {
    public  class UpdateProceduralUIWidthMessage(
        int entityID,
        int width
    ) : Message {
        public delegate void Delegate(
            int entityID,
            int width
        );

        public int EntityID = entityID;
        public int Width = width;
        public override void Send() {
            Messenger<Delegate>.Trigger?.Invoke(
                EntityID,
                Width
            );
        }
    }
}
