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
    public class DirectInputBinds() {
        public Dictionary<MouseOffset, CreateMouseMessage> MouseBinds = [];
        public Dictionary<Key, CreateKeyboardMessage> KeyboardBinds = [];
        public Dictionary<
            JoystickOffset,
            CreateJoystickMessage
        > JoystickBinds = [];
        public bool IsMouseOffsetBound(MouseOffset offset) {
            return MouseBinds.ContainsKey(offset);
        }
        public bool IsKeyboardKeyBound(Key key) {
            return KeyboardBinds.ContainsKey(key);
        }
        public bool IsJoystickOffsetBound(JoystickOffset offset) {
            return JoystickBinds.ContainsKey(offset);
        }
        public IMessage CreateMouseMessage(
            InputDevice device,
            MouseUpdate update
        ) {
            return MouseBinds[update.Offset](device, update);
        }
        public IMessage CreateKeyboardMessage(
            InputDevice device,
            KeyboardUpdate update
        ) {
            return KeyboardBinds[update.Key](device, update);
        }
        public IMessage CreateJoystickMessage(
            InputDevice device,
            JoystickUpdate update
        ) {
            return JoystickBinds[update.Offset](device, update);
        }
        public void SetMouseBind(
            MouseOffset offset,
            CreateMouseMessage createMouseMessage
        ) {
            MouseBinds.Remove(offset);
            MouseBinds.Add(offset, createMouseMessage);
        }
        public void SetKeyboardBind(
            Key key,
            CreateKeyboardMessage createKeyboardMessage
        ) {
            KeyboardBinds.Remove(key);
            KeyboardBinds.Add(key, createKeyboardMessage);
        }
        public void SetJoystickBind(
            JoystickOffset offset,
            CreateJoystickMessage createJoystickMessage
        ) {
            JoystickBinds.Remove(offset);
            JoystickBinds.Add(offset, createJoystickMessage);
        }
    }
}
