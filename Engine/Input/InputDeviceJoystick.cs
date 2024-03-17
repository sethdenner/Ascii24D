using SharpDX.DirectInput;

namespace Engine.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class InputDeviceJoystick : InputDeviceDirectInput
    {
        /// <summary>
        /// 
        /// </summary>
        public InputDeviceJoystick()
        {
            _joystick = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directInput"></param>
        /// <param name="device"></param>
        public InputDeviceJoystick(IDirectInput directInput, IDeviceInstance device) : base(directInput, device)
        {
            _joystick = new Joystick(directInput.SharpDXDirectInput, device.GetInstanceGuid());
            _joystick.Properties.BufferSize = 128;
            _joystick.Acquire();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public virtual JoystickUpdate[] GetUpdates()
        {
            if (null == _joystick)
                throw new NullReferenceException(nameof(_joystick));

            _joystick.Poll();
            return _joystick.GetBufferedData();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DeviceType GetDeviceType()
        {
            return DeviceType.Joystick;
        }
        /// <summary>
        /// 
        /// </summary>
        private Joystick? _joystick;
    }
}
