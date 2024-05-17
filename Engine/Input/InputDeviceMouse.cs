using SharpDX.DirectInput;

namespace Engine.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class InputDeviceMouse : InputDeviceDirectInput
    {
        /// <summary>
        /// 
        /// </summary>
        public InputDeviceMouse()
        {
            _mouse = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directInput"></param>
        /// <param name="device"></param>
        public InputDeviceMouse(
            IDirectInput directInput,
            IDeviceInstance device
        ) : base(directInput, device)
        {
            _mouse = new Mouse(directInput.SharpDXDirectInput);
            _mouse.Properties.BufferSize = 128;
            _mouse.Acquire();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public virtual MouseUpdate[] GetUpdates()
        {
            if (null == _mouse)
                throw new NullReferenceException(nameof(_mouse));

            _mouse.Poll();
            return _mouse.GetBufferedData();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DeviceType GetDeviceType()
        {
            return DeviceType.Mouse;
        }
        /// <summary>
        /// 
        /// </summary>
        private Mouse? _mouse;
    }
}
