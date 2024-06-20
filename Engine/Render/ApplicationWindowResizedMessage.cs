using Engine.Core;
using Engine.Native;

namespace Engine.Render {
    public class ApplicationWindowResizedMessage(
        SMALL_RECT newSizeRect
    ) : Message {
        public delegate void Delegate(SMALL_RECT newSizeRect);
        public SMALL_RECT NewSizeRect = newSizeRect;
        public override void Send() {
            Messenger<Delegate>.Trigger?.Invoke(
                NewSizeRect
            );
        }
    }
}
