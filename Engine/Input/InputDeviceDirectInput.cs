using System;
using SharpDX.DirectInput;

namespace Engine.Input
{
    public abstract class InputDeviceDirectInput : InputDevice
    {
        public InputDeviceDirectInput() 
        {
            _deviceInstance = null;
            _directInput = null;
        }
        public InputDeviceDirectInput(IDirectInput directInput, IDeviceInstance device) : base()
        {
            _deviceInstance = device;
            _directInput = directInput;
        }

        public override Guid DeviceGuid
        {
            get
            {
                if (null != _deviceInstance)
                {
                    return _deviceInstance.GetInstanceGuid();
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }
        private IDeviceInstance? _deviceInstance;
        protected IDirectInput? _directInput;
    }
}
