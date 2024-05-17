namespace Engine.Input
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class InputDeviceDirectInput : InputDevice
    {
        /// <summary>
        /// 
        /// </summary>
        public InputDeviceDirectInput() 
        {
            _deviceInstance = null;
            _directInput = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directInput"></param>
        /// <param name="device"></param>
        public InputDeviceDirectInput(IDirectInput directInput, IDeviceInstance device) : base()
        {
            _deviceInstance = device;
            _directInput = directInput;
        }
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        private IDeviceInstance? _deviceInstance;
        /// <summary>
        /// 
        /// </summary>
        protected IDirectInput? _directInput;
    }
}
