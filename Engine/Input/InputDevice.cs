using SharpDX.DirectInput;

namespace Engine.Input
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class InputDevice
    {
        /// <summary>
        /// 
        /// </summary>
        public InputDevice() { }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract DeviceType GetDeviceType();
        /// <summary>
        /// 
        /// </summary>
        public abstract Guid DeviceGuid { get; }
    }
}
