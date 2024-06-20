using Engine.Core;
using SharpDX.DirectInput;

namespace Engine.Input {
    public class MouseMessage(
        InputDevice device,
        MouseUpdate update
    ) : Message {
        public delegate void Delegate(
            InputDevice device,
            MouseUpdate update
        );

        InputDevice Device = device;
        MouseUpdate Update = update;

        public override void Send() {
            Messenger<Delegate>.Trigger?.Invoke(
                Device,
                Update
            );
        }
    }
}
