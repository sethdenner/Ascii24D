using Engine.Core;
using SharpDX.DirectInput;

namespace Engine.Input {
    public class JoystickMessage(
        InputDevice device,
        JoystickUpdate update
    ) : Message {
        public delegate void Delegate(
            InputDevice device,
            JoystickUpdate update
        );

        public InputDevice Device = device;
        public JoystickUpdate Update = update;

        public override void Send() {
            Messenger<Delegate>.Trigger?.Invoke(
                Device,
                Update
            );
        }
    }
}
