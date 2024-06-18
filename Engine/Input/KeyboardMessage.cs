using Engine.Core;
using SharpDX.DirectInput;

namespace Engine.Input {
    public class KeyboardMessage(
        InputDevice device,
        KeyboardUpdate update
    ) : Message {
        public delegate void KeyboardMessageDelegate(
            InputDevice device,
            KeyboardUpdate update
        );

        public InputDevice Device = device;
        public KeyboardUpdate Update = update;

        public override void Send() {
            Messenger<KeyboardMessageDelegate>.Trigger?.Invoke(
                Device,
                Update
            );
        }
    }
}
