using SharpDX.DirectInput;

namespace Engine.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class InputDeviceKeyboard : InputDeviceDirectInput
    {
        /// <summary>
        /// 
        /// </summary>
        public InputDeviceKeyboard()
        {
            _keyboard = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directInput"></param>
        /// <param name="device"></param>
        public InputDeviceKeyboard(IDirectInput directInput, IDeviceInstance device) : base(directInput, device)
        {
            _keyboard = new Keyboard(directInput.SharpDXDirectInput);
            _keyboard.Properties.BufferSize = 128;
            _keyboard.Acquire();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public virtual KeyboardUpdate[] GetUpdates()
        {
            if (null == _keyboard)
                throw new NullReferenceException(nameof(_keyboard));

            _keyboard.Poll();
            return _keyboard.GetBufferedData();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DeviceType GetDeviceType()
        {
            return DeviceType.Keyboard;
        }
        /// <summary>
        /// 
        /// </summary>
        private Keyboard? _keyboard;
    }
}
