using SharpDX.DirectInput;

namespace Engine.Input
{
    public class InputDeviceJoystick : InputDeviceDirectInput
    {
        public InputDeviceJoystick()
        {
            _joystick = null;
        }
        public InputDeviceJoystick(IDirectInput directInput, IDeviceInstance device) : base(directInput, device)
        {
            _joystick = new Joystick(directInput.SharpDXDirectInput, device.GetInstanceGuid());
            _joystick.Properties.BufferSize = 128;
            _joystick.Acquire();
        }

        public JoystickUpdate[] GetUpdates()
        {
            if (null == _joystick)
                throw new NullReferenceException(nameof(_joystick));

            _joystick.Poll();
            return _joystick.GetBufferedData();
        }

        public override DeviceType GetDeviceType()
        {
            return DeviceType.Joystick;
        }

        private Joystick? _joystick;
    }
}
