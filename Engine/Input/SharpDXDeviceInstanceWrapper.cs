using SharpDX.DirectInput;
using System.Runtime.CompilerServices;


namespace Engine.Input
{
    public interface IDeviceInstance
    {
        public DeviceType GetDeviceType();
        public Guid GetInstanceGuid();
        public DeviceInstance SharpDXDeviceInstance { get; set; }
    }

    public class SharpDXDeviceInstanceWrapper : IDeviceInstance
    {
        public SharpDXDeviceInstanceWrapper(DeviceInstance sharpDXDeviceInstance)
        {
            SharpDXDeviceInstance = sharpDXDeviceInstance;
        }
        public virtual DeviceType GetDeviceType()
        {
            return SharpDXDeviceInstance.Type;
        }

        public virtual Guid GetInstanceGuid()
        {
            return SharpDXDeviceInstance.InstanceGuid;
        }
        public DeviceInstance SharpDXDeviceInstance { get; set; }
    }
}
