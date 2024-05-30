using Engine.Core;
using SharpDX.DirectInput;

namespace Engine.Input
{
    public delegate IMessage CreateMouseMessage(
        InputDevice device,
        MouseUpdate update
    );
    public delegate IMessage CreateKeyboardMessage(
        InputDevice device,
        KeyboardUpdate update
    );
    public delegate IMessage CreateJoystickMessage(
        InputDevice device,
        JoystickUpdate update
    );
    /// <summary>
    /// <c>DirectInputBinds</c> manages emitting user defined messages on user
    /// configured input messages.
    /// </summary>
    public class DirectInputBinds
    {
        /// <summary>
        /// <c>InputBinds</c> default constructor.
        /// </summary>
        public DirectInputBinds()
        {
        }
        public CreateMouseMessage[] MouseBinds = new CreateMouseMessage[
            (int)MouseOffset.Buttons7 + 1
        ];
        public CreateKeyboardMessage[] KeyboardBinds = new CreateKeyboardMessage[
            (int)Key.MediaSelect + 1
        ];
        public CreateJoystickMessage[] JoystickBinds = new CreateJoystickMessage[
            (int)JoystickOffset.ForceSliders1 + 1
        ];
        public bool IsMouseOffsetBound(MouseOffset offset) {
            return null != MouseBinds[(int)offset];
        }
        public bool IsKeyboardKeyBound(Key key) {
            return null != KeyboardBinds[(int)key];
        }
        public bool IsJoystickOffsetBound(JoystickOffset offset) {
            return null != JoystickBinds[(int)offset];
        }
        public IMessage CreateMessageFromMouse(
            InputDevice device,
            MouseUpdate update
        ) {
            return MouseBinds[(int)update.Offset](device, update);
        }
        public IMessage CreateMessageFromKeyboard(
            InputDevice device,
            KeyboardUpdate update
        ) {
            return KeyboardBinds[(int)update.Key](device, update);
        }
        public IMessage CreateMessageFromJoystick(
            InputDevice device,
            JoystickUpdate update
        ) {
            return JoystickBinds[(int)update.Offset](device, update);
        }
    }
}
