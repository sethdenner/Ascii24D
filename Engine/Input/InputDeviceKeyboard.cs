using SharpDX.DirectInput;

namespace Engine.Input
{
    public class InputDeviceKeyboard : InputDeviceDirectInput
    {
        public InputDeviceKeyboard()
        {
            _keyboard = null;
        }
        public InputDeviceKeyboard(IDirectInput directInput, IDeviceInstance device) : base(directInput, device)
        {
            _keyboard = new Keyboard(directInput.SharpDXDirectInput);
            _keyboard.Properties.BufferSize = 128;
            _keyboard.Acquire();
        }
        public KeyboardUpdate[] GetUpdates()
        {
            if (null == _keyboard)
                throw new NullReferenceException(nameof(_keyboard));

            _keyboard.Poll();
            return _keyboard.GetBufferedData();
        }
        public override DeviceType GetDeviceType()
        { return DeviceType.Keyboard; }
        private Keyboard? _keyboard;
    }
}
