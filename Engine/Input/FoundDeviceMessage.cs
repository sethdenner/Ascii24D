using Engine.Core;
using SharpDX.DirectInput;

namespace Engine.Input {
    public class FoundDeviceMessage(
        Guid deviceGuid,
        DeviceType deviceType
    ) : Message {
        public delegate void Delegate(
            Guid deviceGuid,
            DeviceType deviceType
        );

        Guid DeviceGuid = deviceGuid;
        DeviceType DeviceType = deviceType;
        public override void Send() {
            Messenger<Delegate>.Trigger?.Invoke(
                DeviceGuid,
                DeviceType
            );
        }
    }
}
