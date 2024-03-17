using SharpDX.DirectInput;

namespace Engine.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class InputDeviceMouse : InputDeviceDirectInput
    {
        public InputDeviceMouse()
        {
            _mouse = null;
        }

        public InputDeviceMouse(IDirectInput directInput, IDeviceInstance device) : base(directInput, device)
        {
            _mouse = new Mouse(directInput.SharpDXDirectInput);
            _mouse.Properties.BufferSize = 128;
            _mouse.Acquire();

        }
        public virtual MouseUpdate[] GetUpdates()
        {
            if (null == _mouse)
                throw new NullReferenceException(nameof(_mouse));

            _mouse.Poll();
            return _mouse.GetBufferedData();
        }
        public override DeviceType GetDeviceType()
        {
            return DeviceType.Mouse;
        }
        private Mouse? _mouse;
    }
}
