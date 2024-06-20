using Engine.Core;

namespace Engine.Characters.UI {
    public class UpdateProceduralUITextMessage(
        int entityID,
        string text
    ) : Message {
        public delegate void Delegate(
            int entityID,
            string text
        );

        public int EntityID = entityID;
        public string Text = text;
        public override void Send() {
            Messenger<Delegate>.Trigger?.Invoke(
                EntityID,
                Text
            );
        }
    }
}
