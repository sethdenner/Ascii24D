using SharpDX.DirectInput;

namespace Engine.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class SharpDXDeviceInstanceWrapper : IDeviceInstance
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sharpDXDeviceInstance"></param>
        public SharpDXDeviceInstanceWrapper(DeviceInstance sharpDXDeviceInstance)
        {
            SharpDXDeviceInstance = sharpDXDeviceInstance;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual DeviceType GetDeviceType()
        {
            return SharpDXDeviceInstance.Type;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Guid GetInstanceGuid()
        {
            return SharpDXDeviceInstance.InstanceGuid;
        }
        /// <summary>
        /// 
        /// </summary>
        public DeviceInstance SharpDXDeviceInstance { get; set; }
    }
}
