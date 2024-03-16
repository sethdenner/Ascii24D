using SharpDX.DirectInput;

namespace Engine.Input
{
    public interface IInputDevice
    {
        public DeviceType GetDeviceType();
        public Guid DeviceGuid { get; }
    }
    public abstract class InputDevice : IInputDevice
    {
        public InputDevice() { }
        public abstract DeviceType GetDeviceType();
        public abstract Guid DeviceGuid { get; }
    }
}
